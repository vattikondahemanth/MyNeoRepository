using Newtonsoft.Json;
using SBIMF_ESOA_CoreEntities;
using SBIMF_ESOA_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_BAL
{
    public class LoginBAL
    {
        private static IGenericDAL<UserDetails> _obj;
        List<Address> lst_Address = new List<Address>();
        UserDetails userD = new UserDetails();
        List<SummaryTotal> lst_SummaryTotal = new List<SummaryTotal>();
        ErrorLogBAL errLogBAL = new ErrorLogBAL();
        List<FundCategoryEntity> lstFunCatValue = new List<FundCategoryEntity>();
        List<ReportScheme> lst_ReportScheme = new List<ReportScheme>();
        List<Trans> lst_Trans = new List<Trans>();

        public LoginBAL() : this(new GenericDAL<UserDetails> ()) 
        {}

        public LoginBAL(GenericDAL<UserDetails> on)
        {
            _obj = on;
        }

        public DataSet GetAcctStmt(string folioNo, string dateFrom, string dateTo)
        {
            string obj_acc = _obj._serviceObject.Account_Statement_New(folioNo, "", dateFrom, dateTo);
            DataSet ds = new DataSet();
            ds = Common.JsonToClass<DataSet>(obj_acc);
            return ds;
        }

        public UserDetails AuthenticateUser(string folioNo, string Password)
        {
            try
            {
                //------------Financial year start date calculation --------------------
                DateTime fyStartDate, currDate = DateTime.Today;
                string dtFrom, dtTo;

                int month, year;
                month = Convert.ToInt32(currDate.Month);
                year = Convert.ToInt32(currDate.Year);

                if (month < 4)
                {
                    fyStartDate = Convert.ToDateTime("01-Apr-" + (year - 1));
                }
                else
                {
                    fyStartDate = Convert.ToDateTime("01-Apr-" + year);
                }

                dtFrom = fyStartDate.ToString("dd-MMM-yyyy");
                dtTo = currDate.ToString("dd-MMM-yyyy");
                //------------End: Financial year start date calculation --------------------


                DataSet ds = GetAcctStmt(folioNo, dtFrom, dtTo);

                if (ds.Tables.Contains("Accountstatement"))
                {
                    if (ds.Tables["Accountstatement"].Rows[0]["Return_Code"].ToString() == "0")
                    {
                        lst_Address = Common.GetResultType<List<Address>>(ds.Tables["in_address"]);
                        lst_SummaryTotal = Common.GetResultType<List<SummaryTotal>>(ds.Tables["summarytotal"]);

                        if (lst_Address[0].panno == Password)
                        {
                            userD.successflag = true;
                            userD.userName = lst_Address[0].name;
                            userD.folioNo = lst_Address[0].folio;
                            userD.PAN = lst_Address[0].panno;
                            string[] email = (lst_Address[0].email).Split(':');
                            userD.email = email[1].Substring(1).ToString();
                            userD.phoneNo = lst_Address[0].phoneres;
                            userD.lstSummaryTotal = lst_SummaryTotal;

                            //--------------Investment Summary from financial start date to current date--------------
                            //Save scheme details
                            lst_ReportScheme = Common.JsonToClass<List<ReportScheme>>(JsonConvert.SerializeObject(ds.Tables["scheme"]));

                            //Get fund category details for schemes
                            var scheme_codes = lst_ReportScheme.ToList().Select(x => x.Code).ToArray();

                            SqlCommand cmd = new SqlCommand("Usp_GetFundCategorySchemeCodeMap");
                            cmd.Parameters.AddWithValue("@SchemeCodes", String.Join(",", scheme_codes));

                            DataSet ds_FundCat = _obj.GetDataSetExecuteStoredProc(cmd);
                            List<SchemeEntity> lst_Scheme = Common.JsonToClass<List<SchemeEntity>>(JsonConvert.SerializeObject(ds_FundCat.Tables[0]));

                            //end: Get fund category details for schemes

                            lst_ReportScheme.RemoveAll(x => Convert.ToDecimal(x.costvalue) == 0);

                            //Calculate gain & loss 
                            lst_ReportScheme.RemoveAll(x => Convert.ToDecimal(x.costvalue) == 0);
                            for (int i = 0; i < lst_ReportScheme.Count; i++)
                            {
                                int index = lst_ReportScheme[i].markvalue.IndexOf(":");
                                string value = lst_ReportScheme[i].markvalue.Substring(index + 1);

                                lst_ReportScheme[i].currentvalue = Convert.ToDecimal(value);

                                if (Convert.ToDecimal(lst_ReportScheme[i].costvalue) == 0)
                                {
                                    lst_ReportScheme[i].gain_loss = "Gain/Loss";
                                    lst_ReportScheme[i].gain_loss_value = 0;
                                }
                                else if (Convert.ToDecimal(lst_ReportScheme[i].costvalue) < Convert.ToDecimal(lst_ReportScheme[i].currentvalue))
                                {
                                    lst_ReportScheme[i].gain_loss = "Gain";
                                    lst_ReportScheme[i].gain_loss_value = Math.Round(((Convert.ToDecimal(lst_ReportScheme[i].currentvalue) - Convert.ToDecimal(lst_ReportScheme[i].costvalue)) * 100) / Convert.ToDecimal(lst_ReportScheme[i].costvalue), 2);
                                }
                                else
                                {
                                    lst_ReportScheme[i].gain_loss = "Loss";
                                    lst_ReportScheme[i].gain_loss_value = Math.Round(((Convert.ToDecimal(lst_ReportScheme[i].costvalue) - Convert.ToDecimal(lst_ReportScheme[i].currentvalue)) * 100) / Convert.ToDecimal(lst_ReportScheme[i].costvalue), 2);
                                }
                                lst_ReportScheme[i].Name = lst_ReportScheme[i].Lname.Substring((lst_ReportScheme[i].Code.Length));
                                lst_ReportScheme[i].FundCategoryId = Convert.ToInt32(lst_Scheme.ToList().Where(x => x.Scheme_code == (lst_ReportScheme[i].Code)).Select(y => y.FundCategoryId).FirstOrDefault());
                                lst_ReportScheme[i].CategoryName = Convert.ToString(lst_Scheme.ToList().Where(x => x.Scheme_code == (lst_ReportScheme[i].Code)).Select(y => y.CategoryName).FirstOrDefault());

                                //lst_ReportScheme[i].schemetransact = lst_Trans.ToList().Where(x => x.Code == lst_ReportScheme[i].Code).ToList();

                            }
                            //End: Save scheme details

                            //Generate fund category wise scheme details
                            lstFunCatValue = (from r in lst_ReportScheme.ToList()
                                              group r by r.FundCategoryId into g
                                              select new FundCategoryEntity
                                              {
                                                  FundCategoryId = g.First().FundCategoryId,
                                                  CategoryName = g.First().CategoryName,
                                                  Invest_Amt = g.Sum(h => decimal.Parse(h.costvalue)),
                                                  Current_Value = g.Sum(h => h.currentvalue),
                                                  Scheme_Value = Math.Round((g.Sum(h => decimal.Parse(h.costvalue)) != 0 ?
                                                                  ((g.Sum(h => h.currentvalue) > g.Sum(h => decimal.Parse(h.costvalue))) ?
                                                                  ((g.Sum(h => h.currentvalue) - g.Sum(h => decimal.Parse(h.costvalue))) * 100 / g.Sum(h => decimal.Parse(h.costvalue)))
                                                                  : ((g.Sum(h => decimal.Parse(h.costvalue)) - g.Sum(h => h.currentvalue)) * 100 / g.Sum(h => decimal.Parse(h.costvalue))))
                                                                  : 0), 2),
                                                  gain_loss_tooltip = (g.Sum(h => decimal.Parse(h.costvalue)) != 0 ?
                                                                  ((g.Sum(h => h.currentvalue) > g.Sum(h => decimal.Parse(h.costvalue))) ? "Gain" : "Loss")
                                                                  : "Gain/Loss"),
                                                  lst_Scheme = lst_ReportScheme.ToList().Where(i => i.FundCategoryId == (g.First().FundCategoryId)).ToList()
                                              }).ToList();

                            lstFunCatValue[0].CustomerName = ds.Tables["in_address"].Rows[0]["name"].ToString();
                            lstFunCatValue[0].panNo = ds.Tables["in_address"].Rows[0]["panno"].ToString();

                            //End: ---------------Investment Summary from financial start date to current date--------------

                            userD.lstFundCat = lstFunCatValue;
                            userD.lstFundCatTrans = Common.JsonToClass<List<Trans>>(JsonConvert.SerializeObject(ds.Tables["trans"]));

                        }
                        else
                        {
                            userD.successflag = false;
                        }
                    }
                    else
                    {
                        userD.error = "Error Code: " + ds.Tables["Accountstatement"].Rows[0]["Return_Code"].ToString() + ", Error Message: " + ds.Tables["Accountstatement"].Rows[0]["Return_Msg"].ToString();
                        errLogBAL.InsertErroLog("AuthenticateUser", "Login", "Return Code: " + ds.Tables["Accountstatement"].Rows[0]["Return_Code"].ToString(), userD.error);
                    }
                }
                else if (ds.Tables.Contains("information"))
                {
                    userD.error = "Error Code: " + ds.Tables["information"].Rows[0]["Return_Code"].ToString() + ", Error Message: " + ds.Tables["information"].Rows[0]["Return_Msg"].ToString();
                    errLogBAL.InsertErroLog("AuthenticateUser", "Login", "Return Code: " + ds.Tables["information"].Rows[0]["Return_Code"].ToString(), userD.error);
                }
            }
            catch (Exception ex)
            {
                userD.error = ex.Message.ToString();
                errLogBAL.InsertErroLog("AuthenticateUser", "Login", ex.Message, ex.StackTrace);
            }

            return userD;
        }

        public void ExpireURL(string URL)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("usp_ESOAUrlExpired");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@URL", URL);
                _obj.InsertData(cmd);
            }
            catch(Exception ex)
            {
                errLogBAL.InsertErroLog("ExpireURL", "Login", ex.Message, ex.StackTrace);
            }
        }

        public string CheckLoginLog(string URL)
        {
            string message = "";
            try
            {
                SqlCommand cmd = new SqlCommand("usp_CreateESOALoginLog");
                cmd.Parameters.AddWithValue("@URL", URL);
                DataSet ds = _obj.GetDataSetExecuteStoredProc(cmd);
                if(ds.Tables.Count > 0)
                {
                    message = ds.Tables[0].Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                errLogBAL.InsertErroLog("ExpireURL", "Login", ex.Message, ex.StackTrace);
            }
            return message;
        }

        public string CheckLoginAttempts(string URL, bool ValidUser)
        {
            string message = "";
            try
            {
                SqlCommand cmd = new SqlCommand("usp_ESOALoginInvalidAttempt");
                cmd.Parameters.AddWithValue("@URL", URL);
                cmd.Parameters.AddWithValue("@ValidUser", ValidUser);
                DataSet ds = _obj.GetDataSetExecuteStoredProc(cmd);
                if (ds.Tables.Count > 0)
                {
                    message = ds.Tables[0].Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                errLogBAL.InsertErroLog("ExpireURL", "Login", ex.Message, ex.StackTrace);
            }
            return message;
        }
    }
}
