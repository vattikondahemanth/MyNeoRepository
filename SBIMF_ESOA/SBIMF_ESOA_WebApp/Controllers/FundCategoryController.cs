using GenerateAccountStatementDLL;
using Newtonsoft.Json;
using SBIMF_ESOA_BAL;
using SBIMF_ESOA_CoreEntities;
using SBIMF_ESOA_WebApp.Filters;
using SBIMF_ESOA_WebApp.Models;
using SBIMF_ESOA_WebApp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SBIMF_ESOA_WebApp.Controllers
{
    [SessionTimeout]
    [EncryptedActionParameterAttribute]
    [Filters.HandleError]
    public class FundCategoryController : Controller
    {
        private GetFundCategoryDetailsBAL IBal;

        public FundCategoryController(GetFundCategoryDetailsBAL obj)
        {
            this.IBal = obj;
        }

        public FundCategoryController(): this(new GetFundCategoryDetailsBAL())
        { }
        // GET: FundCategory
        public ActionResult Index()
        {
            return View();
        }

        //Investment summary fund category chart
        public ActionResult Invest_summary()
        {
            return View();
        }

        [AjaxValidateAntiForgeryToken]
        public ActionResult GetChart(string dateFrom, string dateTo)
        {
            var Returnmsg = "";
            DateTime _dateFrom = Convert.ToDateTime(dateFrom);
            DateTime _dateTo = Convert.ToDateTime(dateTo);
            //dateFrom = dateTo = DateTime.Now;
            string key = Session["pass_enc_key"].ToString();
            //Dictionary<string, string> paramdictionary = new Dictionary<string, string>();
            //RequestUtility _requestutility = new RequestUtility();
            //var decryptedrequest = "";

            //if (Request.IsAjaxRequest())
            //{
            //    decryptedrequest = AjaxRequestUtility.DecryptRequest(Request.InputStream, key);
            //    _requestutility.GetRequestParamValues(decryptedrequest, ref paramdictionary, nameof(dateFrom), nameof(dateTo));
            //    dateFrom = Convert.ToDateTime(paramdictionary[nameof(dateFrom)]);
            //    dateTo = Convert.ToDateTime(paramdictionary[nameof(dateTo)]);
            //}

            //validation the difference between dateFrom & dateTo should be less than or equal to one year
            int noOfDays = Convert.ToInt32((_dateTo - _dateFrom).TotalDays);
            string folioNo = Session["Folio_No"].ToString();  //13627577

            if (noOfDays > 365)
            {
                Returnmsg = JsonConvert.SerializeObject((new { ErrMessage = "Please select time period less than or equal to one year." }));
            }
            else
            {
                FundCatTransactions fundCatTrans= new FundCatTransactions();
                List<FundCategoryEntity> lstFundCat = new List<FundCategoryEntity>();

                //------------Financial year start date calculation --------------------
                DateTime fyStartDate, currDate = DateTime.Today;

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
                //------------End: Financial year start date calculation --------------------
                if (fyStartDate.Date == _dateFrom.Date && currDate.Date == _dateTo.Date)
                {
                     lstFundCat = (List<FundCategoryEntity>)TempData.Peek("FundCatChartDt");
                     TempData.Keep("FundCatChartDt");
                     TempData["schTrans"] = TempData.Peek("schTransDt");
                     TempData.Keep("schTransDt");
                 }
                else
                 {
                     fundCatTrans = IBal.GenerateFundCategoryChart(folioNo, _dateFrom.ToString("dd-MMM-yyyy"), _dateTo.ToString("dd-MMM-yyyy"));
                     lstFundCat = fundCatTrans.fundCatEntity;
                     TempData["schTrans"] = fundCatTrans.lstTrans;
                 }

                if (lstFundCat == null)
                {
                    Returnmsg = JsonConvert.SerializeObject((new { ErrMessage = "No Data" }));
                }
                else
                {
                    Returnmsg = JsonConvert.SerializeObject(lstFundCat);
                }                
            }
            Returnmsg = AjaxRequestUtility.EncryptStringAES(Returnmsg, key);
            return Json(new { ResponseBody = Returnmsg }, JsonRequestBehavior.AllowGet);
        }

        //Download account statement
        public ActionResult Account_Statement()
        {
            return View();
        }

        [AjaxValidateAntiForgeryToken]
        public ActionResult GenerateAcctStmtPdf(string dateFrom, string dateTo) //string folioNo, DateTime dateFrom, DateTime dateTo
        {
            string key = Session["Pass_Enc_Key"].ToString();
            var Returnmsg = "";
            //DateTime _dateFrom , _dateTo;
            //_dateFrom = _dateTo= DateTime.Now;
            //Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
            //RequestUtility _requestUtility = new RequestUtility();
            //var DecryptedRequest = "";

            //if (Request.IsAjaxRequest())
            //{
            //    DecryptedRequest = AjaxRequestUtility.DecryptRequest(Request.InputStream, key);
            //    _requestUtility.GetRequestParamValues(DecryptedRequest, ref ParamDictionary, nameof(_dateFrom), nameof(dateTo));
            //    dateFrom = Convert.ToDateTime(ParamDictionary[nameof(dateFrom)]);
            //    dateTo = Convert.ToDateTime(ParamDictionary[nameof(dateTo)]);
            //}

            DateTime _dateFrom = Convert.ToDateTime(dateFrom);
            DateTime _dateTo = Convert.ToDateTime(dateTo);

            string folioNo = Session["Folio_No"].ToString();
            //validation the difference between dateFrom & dateTo should be less than or equal to one year
            int noOfDays = Convert.ToInt32((_dateTo - _dateFrom).TotalDays);

            string[] result;
            byte[] bytes;
            string res = IBal.GenPDF(folioNo, _dateFrom.ToString("dd-MMM-yyyy"), _dateTo.ToString("dd-MMM-yyyy"),out bytes);
            //byte[] bytes = IBal.GenerateAcctStmtPdf(folioNo, dateFrom.ToString("dd-MMM-yyyy"), dateTo.ToString("dd-MMM-yyyy"));

            result = res.Split('`');

            if (noOfDays > 365)
            {
                Returnmsg = JsonConvert.SerializeObject((new { res = "Fail", ErrMessage = "Please select time period less than or equal to one year." }));
                
            }
            else if (result[1] == "00")
            {
                TempData["acct_stmt_bytes"] = bytes;
                Returnmsg = JsonConvert.SerializeObject((new { res = "Success" }));
            }
            else
            {
                Returnmsg = JsonConvert.SerializeObject((new { res = "Error", ErrMessage = "Sorry! An error occurred while downloading account statement..." }));
            }
            Returnmsg = AjaxRequestUtility.EncryptStringAES(Returnmsg, key);
            return Json(new { ResponseBody = Returnmsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadStmt()
        {
            byte[] bytes = (byte[])(TempData["acct_stmt_bytes"]);
            string password = Session["PAN_No"] != null ? Session["PAN_No"].ToString() : "password";
            PDFEncryptor _pdfPasswordProtecttor = new PDFEncryptor();

            using (MemoryStream input = new MemoryStream(bytes))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    _pdfPasswordProtecttor.PasswordProtectPDF(bytes, output, password);
                    bytes = output.ToArray();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=AccountStatement.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(bytes);
                    Response.End();
                }
            }
            return File(bytes, "application/pdf", "AccountStatement.pdf");
        }


        public ActionResult GetAccounttStmtDetails()
        {
            return View();
        }

        //public ActionResult GetAcctStmtDetails(string folioNo, DateTime dateFrom, DateTime dateTo)
        //{
        //    folioNo = Session["Folio_No"].ToString();
        //    AccountStatement obj_acct = IBal.GetAccStmtDetails(folioNo, dateFrom, dateTo);
        //    List<SchemeSummary> obj_lst_scheme = IBal.GetSchemeSummary(folioNo, dateFrom, dateTo);
        //    return Json(obj_acct, JsonRequestBehavior.AllowGet);
        //    //return Json(obj_lst_scheme, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult PieChart_FundCat()
        {
            return View();
        }        

        public ActionResult Test()
        {
            return View();
        }
    }
}