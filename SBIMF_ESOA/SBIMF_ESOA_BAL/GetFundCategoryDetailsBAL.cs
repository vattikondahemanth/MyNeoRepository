using GenerateAccountStatementDLL;
//using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SBIMF_ESOA_CoreEntities;
using SBIMF_ESOA_DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SBIMF_ESOA_BAL  
{
    public class GetFundCategoryDetailsBAL
    {
        AccessSession _objSession = new AccessSession();
        private static IGenericDAL<FundCategoryEntity> _obj;

        static string _ReportSave = Convert.ToString(ConfigurationManager.AppSettings["ReportSaveText"]);
        static string _ReportPath = Convert.ToString(ConfigurationManager.AppSettings["ReportPathText"]);
        
        ErrorLogBAL errLogBAL = new ErrorLogBAL();

        List<FundCategoryEntity> lstFunCatValue = new List<FundCategoryEntity>();
        List<ReportScheme> lst_ReportScheme = new List<ReportScheme>();
        List<Trans> lst_Trans = new List<Trans>();
        FundCatTransactions fund_cat_trans = new FundCatTransactions();
        List<Address> lst_Address = new List<Address>();
        List<ReportPortfolioSummary> lst_ReportPortfolioSummary = new List<ReportPortfolioSummary>();
        List<SummaryTotal> lst_SummaryTotal = new List<SummaryTotal>();

        public GetFundCategoryDetailsBAL() : this(new GenericDAL<FundCategoryEntity> ()) 
        {

        }

        public GetFundCategoryDetailsBAL(GenericDAL<FundCategoryEntity> on)
        {
            _obj = on;
        }

        public DataSet GetAcctStmt(string folioNo, string dateFrom, string dateTo)
        {            
            string obj_acc = _obj._serviceObject.Account_Statement_New(folioNo, "", dateFrom, dateTo);

            DataSet ds = new DataSet();
            ds = JsonConvert.DeserializeObject<DataSet>(obj_acc);

            return ds;
        }

        public FundCatTransactions GenerateFundCategoryChart(string folioNo, string dateFrom, string dateTo)  
        {
            try
            {
                DataSet ds = GetAcctStmt(folioNo, dateFrom, dateTo);

                if (ds.Tables.Contains("Accountstatement"))
                {
                    if (ds.Tables["Accountstatement"].Rows[0]["Return_Code"].ToString() == "0")
                    {
                        lst_Address = Common.JsonToClass<List<Address>>(JsonConvert.SerializeObject(ds.Tables["in_address"]));
                        lst_Trans = Common.JsonToClass<List<Trans>>(JsonConvert.SerializeObject(ds.Tables["trans"]));
                        lst_ReportPortfolioSummary = Common.JsonToClass<List<ReportPortfolioSummary>>(JsonConvert.SerializeObject(ds.Tables["trans1"]));
                        lst_SummaryTotal = Common.JsonToClass<List<SummaryTotal>>(JsonConvert.SerializeObject(ds.Tables["summarytotal"]));
                        lst_Address[0].folio = folioNo;

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
                    }
                }

                fund_cat_trans.fundCatEntity = lstFunCatValue;
                fund_cat_trans.lstTrans = lst_Trans;
                fund_cat_trans.address = lst_Address;
                fund_cat_trans.lstSchemes = lst_ReportScheme;
                fund_cat_trans.lstPortfolioSummary = lst_ReportPortfolioSummary;
                fund_cat_trans.lstSummaryTotal = lst_SummaryTotal;
            }
            catch(Exception ex)
            {
                errLogBAL.InsertErroLog("GenerateFundCategoryChart", "FundCategory", ex.Message, ex.StackTrace);
            }

            return fund_cat_trans;
        }
        

        public string GenPDF(string folioNo, string dateFrom, string dateTo, out byte[] pdffile)
        {
            string Return_Code = "";
            string Return_Msg = "";
            pdffile = null;
            GenerateITextPdf objpdf = new GenerateITextPdf(_ReportPath, _ReportSave);
            try
            {
                DataSet ds = GetAcctStmt(folioNo, dateFrom, dateTo);
                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("DateFrom");
                dt.Columns.Add("DateTo");
                DataRow _dr = dt.NewRow();
                _dr["DateFrom"] = dateFrom;
                _dr["DateTo"] = dateTo;
                dt.Rows.Add(_dr);
                dt.TableName = "Statement_Period";
                ds.Tables.Add(dt);

                //Overall Investment Summary
                string tot_inv_amt=String.Empty, tot_pot_val = String.Empty, tot_gl = String.Empty, gl=String.Empty;
                tot_inv_amt = _objSession.GetSession("Tot_Inv_Amt").ToString();
                tot_pot_val = _objSession.GetSession("Tot_Portfolio_Val").ToString();
                tot_gl = _objSession.GetSession("Gain_Loss").ToString();

                if(Convert.ToDecimal(tot_inv_amt) > Convert.ToDecimal(tot_pot_val)) {  gl = "L";  }
                else if(Convert.ToDecimal(tot_inv_amt) < Convert.ToDecimal(tot_pot_val)) { gl = "G"; }
                else { gl = "N"; }

                DataTable dt_InvSummary = new DataTable();
                dt_InvSummary.Clear();
                dt_InvSummary.Columns.Add("Tot_Inv_Amt");
                dt_InvSummary.Columns.Add("Tot_Port_Val");
                dt_InvSummary.Columns.Add("Tot_GL");
                dt_InvSummary.Columns.Add("GL");
                DataRow _dr_InvSummary = dt_InvSummary.NewRow();
                _dr_InvSummary["Tot_Inv_Amt"] = tot_inv_amt;
                _dr_InvSummary["Tot_Port_Val"] = tot_pot_val;
                _dr_InvSummary["Tot_GL"] = tot_gl;
                _dr_InvSummary["GL"] = gl;
                dt_InvSummary.Rows.Add(_dr_InvSummary);
                dt_InvSummary.TableName = "Overall_Inv_Summary";
                ds.Tables.Add(dt_InvSummary);


                if (ds.Tables.Contains("Accountstatement"))
                {
                    if (ds.Tables["Accountstatement"].Rows[0]["Return_Code"].ToString() == "0")
                    {
                        try
                        {
                            pdffile = objpdf.PrintPDFReport(ds);
                            Return_Code = "00";
                            Return_Msg = "Data Retrieved Successfully.";

                        }
                        catch (Exception ex)
                        {
                            Return_Code = "908";
                            Return_Msg = "CANNOT CREATE FILE Exception : " + ex.Message + " StackTrace : " + ex.StackTrace;

                            errLogBAL.InsertErroLog("GenerateAcctStmtPdf", "FundCategory", ex.Message, Return_Msg);
                        }

                        if (Return_Code.Trim() == "" && Return_Msg.Trim() == "")
                        {
                            Return_Code = ds.Tables["Accountstatement"].Rows[0]["Return_Code"].ToString();
                            Return_Msg = ds.Tables["Accountstatement"].Rows[0]["Return_Msg"].ToString();
                        }
                    }
                }
                if (ds.Tables.Contains("Information"))
                {
                    Return_Code = ds.Tables["Information"].Rows[0]["Return_Code"].ToString();
                    Return_Msg = ds.Tables["Information"].Rows[0]["Return_Msg"].ToString();
                }
            }
            catch (Exception ex)
            {
                Return_Code = "909";
                Return_Msg = "Exception : " + ex.Message + " StackTrace : " + ex.StackTrace;

                errLogBAL.InsertErroLog("GenPDF", "FundCategory", ex.Message, Return_Msg);                 
            }
            return Return_Msg + "`" + Return_Code;
        }




        //public List<FundCategoryEntity> GetSchemeCntByFundCategory(string folioNo, DateTime dateFrom, DateTime dateTo)
        //{    

        //        string obj_Invinfo = _obj._serviceObject.Investor_Information(folioNo, "BUQPS3847C").ToString();
        //        if (!string.IsNullOrEmpty(obj_Invinfo))
        //        {
        //            var tables = JsonConvert.DeserializeObject<DataSet>(obj_Invinfo);
        //            if (tables != null && tables.Tables.Count > 0)
        //            {
        //                var scheme_tbl = JsonConvert.SerializeObject(tables.Tables[4]);

        //                //Get scheme details in Scheme Entity
        //                IEnumerable<SchemeEntity> lst_Scheme = Common.JsonToClass<IEnumerable<SchemeEntity>>(JsonConvert.SerializeObject(tables.Tables[4]));
        //                var scheme_codes = lst_Scheme.ToList().Select(x => x.Scheme_code).ToArray();
        //                //from s in lst_Scheme select s.Scheme_code;

        //                SqlCommand cmd = new SqlCommand("Usp_GetSchemeCntByFundCategory");
        //                cmd.Parameters.AddWithValue("@SchemeCodes", String.Join(",", scheme_codes));

        //                //_objSession.SetSession("scheme_codes", String.Join(",", scheme_codes));
        //                //HttpContext.Current.Session["scheme_codes"] = String.Join(",", scheme_codes);
        //                lstFunCatValue = _obj.ExecuteStoredProc<FundCategoryEntity>(cmd).ToList();
        //                foreach (var item in lstFunCatValue)
        //                {
        //                    item.SchemesList = String.Join(",", scheme_codes);
        //                }
        //            }
        //        }

        //    return lstFunCatValue;
        //}

        //public AccountStatement GetAccStmtDetails(string folioNo, DateTime dateFrom, DateTime dateTo)
        //{
        //    AccountStatement obj_acct = new AccountStatement();

        //    string obj_acc = _obj._serviceObject.Account_Statement_New(folioNo, "", dateFrom.ToString("dd-MMM-yyyy"), dateTo.ToString("dd-MMM-yyyy"));
        //    DataSet ds = new DataSet();
        //    ds = JsonConvert.DeserializeObject<DataSet>(obj_acc);
        //    if (ds.Tables.Contains("Accountstatement"))
        //    {
        //        if (ds.Tables["Accountstatement"].Rows[0]["Return_Code"].ToString() == "0")
        //        {
        //            lst_Address = Common.JsonToClass<List<Address>>(JsonConvert.SerializeObject(ds.Tables["in_address"]));
        //            lst_Trans = Common.JsonToClass<List<Trans>>(JsonConvert.SerializeObject(ds.Tables["trans"]));
        //            lst_ReportScheme = Common.JsonToClass<List<ReportScheme>>(JsonConvert.SerializeObject(ds.Tables["scheme"]));
        //            lst_ReportPortfolioSummary = Common.JsonToClass<List<ReportPortfolioSummary>>(JsonConvert.SerializeObject(ds.Tables["trans1"]));

        //            obj_acct.lst_Address = lst_Address;
        //            obj_acct.lst_Trans = lst_Trans;
        //            obj_acct.lst_ReportScheme = lst_ReportScheme;
        //            obj_acct.lst_ReportPortfolioSummary = lst_ReportPortfolioSummary;  
        //        }
        //    }
        //    return obj_acct;

        //}


        //public List<SchemeSummary> GetSchemeSummary(string folioNo, DateTime dateFrom, DateTime dateTo)
        //{
        //    List<SchemeSummary> obj_lst_scheme = new List<SchemeSummary>();

        //    Address address = new Address();
        //    ReportScheme rptScheme = new ReportScheme();
        //    List<Trans> lst_Sch_Trans = new List<Trans>();
        //    ReportPortfolioSummary rptPortfolioSummary = new ReportPortfolioSummary();

        //    string obj_acc = _obj._serviceObject.Account_Statement_New(folioNo, "", dateFrom.ToString("dd-MMM-yyyy"), dateTo.ToString("dd-MMM-yyyy"));
        //    DataSet ds = new DataSet();
        //    ds = JsonConvert.DeserializeObject<DataSet>(obj_acc);
        //    if (ds.Tables.Contains("Accountstatement"))
        //    {
        //        if (ds.Tables["Accountstatement"].Rows[0]["Return_Code"].ToString() == "0")
        //        {
        //            lst_Address = Common.JsonToClass<List<Address>>(JsonConvert.SerializeObject(ds.Tables["in_address"]));
        //            lst_Trans = Common.JsonToClass<List<Trans>>(JsonConvert.SerializeObject(ds.Tables["trans"]));
        //            lst_ReportScheme = Common.JsonToClass<List<ReportScheme>>(JsonConvert.SerializeObject(ds.Tables["scheme"]));
        //            lst_ReportPortfolioSummary = Common.JsonToClass<List<ReportPortfolioSummary>>(JsonConvert.SerializeObject(ds.Tables["trans1"]));

        //            address = Common.JsonToClass<Address>(JsonConvert.SerializeObject(lst_Address[0]));

        //            for(int i= 0; i< lst_ReportScheme.Count; i++)
        //            {
        //                SchemeSummary obj_scheme = new SchemeSummary();

        //                rptScheme = Common.JsonToClass<ReportScheme>(JsonConvert.SerializeObject(lst_ReportScheme[i]));
        //                //rptScheme.schemetransact = lst_Trans.ToList().Where(x => x.Code == rptScheme.Code).ToList();
        //                //lst_Sch_Trans = rptScheme.schemetransact;
        //                rptPortfolioSummary = lst_ReportPortfolioSummary.ToList().Where(x => x.Code.Substring(1, (rptScheme.Code).Length) == (rptScheme.Code)).FirstOrDefault();

        //                obj_scheme.address = address;
        //                obj_scheme.rptScheme = rptScheme;
        //                obj_scheme.rptSummary = rptPortfolioSummary;
        //                obj_scheme.schTrans = lst_Sch_Trans;
        //                obj_lst_scheme.Add(obj_scheme);
        //            }
        //        }
        //    }
        //    return obj_lst_scheme;

        //}

    }
}
