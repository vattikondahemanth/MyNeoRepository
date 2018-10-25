using Newtonsoft.Json;
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
    public class CalculatorController : Controller
    {
        private CalculatorBAL IBal;
        InvestorPANDetails panDetails = new InvestorPANDetails();

        public CalculatorController(CalculatorBAL obj)
        {
            this.IBal = obj;
        }

        public CalculatorController() : this(new CalculatorBAL())
        { }

        // GET: Calculator
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FamilySolution()
        {
            return View();
        }

        public ActionResult ReturnValueCalculator()
        {
            return View();
        }

        public ActionResult SIPCalculator()
        {
            return View();
        }

        public ActionResult SWPCalculator()
        {
            List<Schemes> sch = IBal.GetSchemes();
            ViewBag.SWPSchemes = new SelectList(sch, "SchemeCode", "SchemeName");
            return View();
        }

        public JsonResult VerifyPAN(string PAN)
        {
            var Returnmsg ="";
            string key = Session["Pass_Enc_Key"].ToString();
            Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
            RequestUtility _requestUtility = new RequestUtility();
            //var DecryptedRequest = "";

            try
            {
                
                //if (Request.IsAjaxRequest())
                //{
                //    DecryptedRequest = AjaxRequestUtility.DecryptRequest(Request.InputStream, key);
                //    _requestUtility.GetRequestParamValues(DecryptedRequest, ref ParamDictionary, nameof(PAN));
                //    PAN = ParamDictionary[nameof(PAN)];

                //}
                panDetails = IBal.CheckKycForInvestorInfo(PAN);
                Returnmsg = JsonConvert.SerializeObject(panDetails);
                Returnmsg = AjaxRequestUtility.EncryptStringAES(Returnmsg, key);
            }
            catch (Exception ex)
            {
                Returnmsg = ex.ToString();
            }
            return Json(new { ResponseBody = Returnmsg }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult EncryptEncode(string queryStringData)
        {
            var Returnmsg= "";
            
            string ReturnCode = String.Empty;
            string Data = string.Empty;
            //Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
            //RequestUtility _requestUtility = new RequestUtility();
            //var DecryptedRequest = "";
            try
            {
                string key = Session["Pass_Enc_Key"].ToString();
                if (Request.IsAjaxRequest())
                {
                   
                    
                    //DecryptedRequest = AjaxRequestUtility.DecryptRequest(Request.InputStream, key);
                    //string[] RequestParams = new string[2] { "dateFrom", "dateTo" };
                    //_requestUtility.GetRequestParamValues(DecryptedRequest, ref ParamDictionary, nameof(queryStringData));
                    //queryStringData = ParamDictionary[nameof(queryStringData)];
                    if (queryStringData != null)
                    {
                        Data = Common.Base64Encode(Common.AES_Encrypt(queryStringData));
                        ReturnCode = "0";
                    }
                    else
                    {
                        ReturnCode = "101";
                    }

                }

                Returnmsg = JsonConvert.SerializeObject(new { ReturnCode = ReturnCode, Data = Data });
                Returnmsg = AjaxRequestUtility.EncryptStringAES(Returnmsg, key);
            }
            catch (Exception ex)
            {
                Returnmsg = ex.ToString();
            }
            return Json(new { ResponseBody = Returnmsg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SBIMFJSONService(string MethodName, string parameters)
        {
            string Returnmsg = "" ;
            
            string ReturnCode = String.Empty;
            string[] StoreProcParams = new string[3];
            string key = Session["Pass_Enc_Key"].ToString();
            if (Request.IsAjaxRequest())
            {
                try
                {


                    //Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
                    //RequestUtility _requestUtility = new RequestUtility();
                    //var DecryptedRequest = "";
                    //DecryptedRequest = AjaxRequestUtility.DecryptRequest(Request.InputStream, key);
                    //string[] RequestParams = new string[2] { "dateFrom", "dateTo" };
                    //_requestUtility.GetRequestParamValues(DecryptedRequest, ref ParamDictionary, nameof(MethodName), nameof(parameters));
                    //MethodName = ParamDictionary[nameof(MethodName)];
                    //parameters = ParamDictionary[nameof(parameters)];
                    if (!string.IsNullOrWhiteSpace(parameters) && parameters != "[]")
                    {
                        var ParamArray = parameters.Replace("\r", "").Replace("\n", "").Replace("[", "").Replace("]", "").Split('\"');
                        StoreProcParams[0] = ParamArray[1];
                        StoreProcParams[1] = ParamArray[3];
                        StoreProcParams[2] = ParamArray[5];
                    }
                    
                    Returnmsg = IBal.SBIMFJSONService(MethodName, StoreProcParams);

                }

                catch (Exception ex)
                {
                    Returnmsg = ex.ToString();
                }


            }
           


            Returnmsg = AjaxRequestUtility.EncryptStringAES(Returnmsg, key);
            return Json(new { ResponseBody = Returnmsg }, JsonRequestBehavior.AllowGet);

        }
    }
}