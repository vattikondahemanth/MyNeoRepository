using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SBIMF_ESOA_BAL;
using SBIMF_ESOA_CoreEntities;
using SBIMF_ESOA_WebApp.Filters;
using SBIMF_ESOA_WebApp.Models;
using SBIMF_ESOA_WebApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBIMF_ESOA_WebApp.Controllers
{
    [SessionTimeout]
    [Filters.HandleError]
    [EncryptedActionParameterAttribute]
    public class SchemeController : Controller
    {
        private SchemeDetailsBAL IBal;

        public SchemeController(SchemeDetailsBAL obj)
        {
            this.IBal = obj;
        }

        public SchemeController(): this(new SchemeDetailsBAL())
        { }
        // GET: Scheme
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SchemeChart()
        {
            return View();
        }

        [AjaxValidateAntiForgeryToken]
        //Generate Scheme Chart
        public ActionResult GetSchemeChart()
        {
            List<ReportScheme> lSchemes = new List<ReportScheme>();
            DateTime dtFrom = DateTime.Now;
            DateTime dtTo = DateTime.Now;
            var Returnmsg = "";
            DateTime dateFrom, dateTo;
            dateFrom = dateTo = DateTime.Now;
            string key = Session["Pass_Enc_Key"].ToString();
            if (Request.IsAjaxRequest())
            {
                Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
                RequestUtility _requestUtility = new RequestUtility();
                var DecryptedRequest = "";
                DecryptedRequest = AjaxRequestUtility.DecryptRequest(Request.InputStream, key);
                var ReportSchemeList = JObject.Parse(DecryptedRequest)["lSchemes"];
                for (int i = 0; i < JObject.Parse(DecryptedRequest)["lSchemes"].Count(); i++)
                {
                    ReportScheme lScheme = new ReportScheme();
                    lScheme.No =  ReportSchemeList[i]["No"].ToString();
                    lScheme.Code = ReportSchemeList[i]["Code"].ToString();
                    lScheme.amccode = ReportSchemeList[i]["amccode"].ToString();
                    lScheme.Lname = ReportSchemeList[i]["Lname"].ToString();
                    lScheme.Name = ReportSchemeList[i]["Name"].ToString();
                    lScheme.costvalue = ReportSchemeList[i]["costvalue"].ToString();
                    lScheme.Opbal = ReportSchemeList[i]["Opbal"].ToString();
                    lScheme.Nav = ReportSchemeList[i]["Nav"].ToString();
                    lScheme.Schremark = ReportSchemeList[i]["Schremark"].ToString();
                    lScheme.ISIN = ReportSchemeList[i]["ISIN"].ToString();
                    lScheme.brokername = ReportSchemeList[i]["brokername"].ToString();
                    lScheme.bankacct = ReportSchemeList[i]["bankacct"].ToString();
                    lScheme.lien_unit = ReportSchemeList[i]["lien_unit"].ToString();
                    lScheme.markvalue = ReportSchemeList[i]["markvalue"].ToString();
                    lScheme.currentvalue = Convert.ToDecimal(ReportSchemeList[i]["currentvalue"].ToString());
                    lScheme.gain_loss_value = Convert.ToDecimal(ReportSchemeList[i]["gain_loss_value"].ToString());
                    lScheme.gain_loss = ReportSchemeList[i]["gain_loss"].ToString();
                    lScheme.FundCategoryId = Convert.ToInt32(ReportSchemeList[i]["FundCategoryId"].ToString());
                    lScheme.CategoryName = ReportSchemeList[i]["CategoryName"].ToString();
                    lSchemes.Add(lScheme);

                }
                dtFrom = Convert.ToDateTime(JObject.Parse(DecryptedRequest)["dtFrom"]);
                dateTo = Convert.ToDateTime(JObject.Parse(DecryptedRequest)["dtTo"]);
            }
            TempData["ReportSchemes"] = lSchemes;
            Session["dtFrom"] = dtFrom.ToString("dd/MMM/yyyy");
            Session["dtTo"] = dtTo.ToString("dd/MMM/yyyy");
            Returnmsg = AjaxRequestUtility.EncryptStringAES(JsonConvert.SerializeObject(lSchemes), key);
            return Json(new { ResponseBody = Returnmsg }, JsonRequestBehavior.AllowGet);

            
        }

        [AjaxValidateAntiForgeryToken]
        public ActionResult SchemeChartData()
        {
            var Returnmsg = "";
            string key = Session["Pass_Enc_Key"].ToString();
            List<ReportScheme> lstSchemes = (List<ReportScheme>)TempData.Peek("ReportSchemes");
            TempData.Keep("ReportSchemes");
            Returnmsg = AjaxRequestUtility.EncryptStringAES(JsonConvert.SerializeObject(lstSchemes), key);
            return Json(new { ResponseBody = Returnmsg }, JsonRequestBehavior.AllowGet);
           
        }

        [AjaxValidateAntiForgeryToken]
        //Get Scheme Details
        public ActionResult GetSchemeD_Chart(string sch_code, float sch_percentage)
        {
            Session["sch_code"] = sch_code;
            Session["sch_percentage"] = sch_percentage;
            return Json(sch_code, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplaySchemeD_Chart()
        {
            ViewBag.Sch_Percentage = Session["sch_percentage"].ToString();
            return View();
        }

        [AjaxValidateAntiForgeryToken]
        public ActionResult DisplaySchemeD_Chart_Data()
        {
            var Returnmsg = "";
            string key = Session["Pass_Enc_Key"].ToString();
            string code = Session["sch_code"].ToString();
            ReportScheme sch_d = ((List<ReportScheme>)TempData.Peek("ReportSchemes")).ToList().Where(x => x.Code == code).FirstOrDefault();
            //ReportScheme sch_d = (ReportScheme)TempData.Peek("sch_d");
            List<Trans> lstTrns = ((List<Trans>)TempData.Peek("schTrans")).ToList().Where(x => x.Code == code).ToList();
            TempData.Keep("schTrans");
            //var SchTransactions = new Tuple<ReportScheme, List<Trans>>(sch_d, lstTrns);
            Returnmsg = AjaxRequestUtility.EncryptStringAES(JsonConvert.SerializeObject(new { lstSchemes = sch_d, lstTrns = lstTrns }), key);
            return Json(new { ResponseBody = Returnmsg }, JsonRequestBehavior.AllowGet);
            

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult SchemeDetails()
        {           
            List<SchemeEntity> lstSchemeCat = (List<SchemeEntity>)TempData.Peek("schemes");
            return View(lstSchemeCat);
        }


        [AjaxValidateAntiForgeryToken]
        public ActionResult GetAllSchemeByFundCategory(int FundCatId, string schemes=null)
        {
            List<SchemeEntity> lstSchemeCat = new List<SchemeEntity>();
            lstSchemeCat = IBal.GetAllSchemesByFundCategory(FundCatId,schemes);
            TempData["schemes"] = lstSchemeCat;
            return Json(lstSchemeCat);
        }

        
        

    }
}