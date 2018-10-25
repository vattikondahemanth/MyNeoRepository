using SBIMF_ESOA_BAL;
using SBIMF_ESOA_CoreEntities;
using SBIMF_ESOA_WebApp.Filters;
using SBIMF_ESOA_WebApp.Models;
using SBIMF_ESOA_WebApp.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SBIMF_ESOA_WebApp.Controllers
{
    [Filters.HandleErrorAttribute]
    [EncryptedActionParameterAttribute]
    public class LoginController : Controller
    {
        static ErrorLogBAL errLogBAL = new ErrorLogBAL();
        UserDetails userD = new UserDetails();
        private LoginBAL IBal;
        public LoginController(LoginBAL obj)
        {
            this.IBal = obj;
        }

        public LoginController() : this(new LoginBAL())
        { }

        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            //string dt1 = DateTime.Now.ToString(); //DateTime.Now.AddDays(-31).Date.ToString();
            //var str = "folioNo=10430675&date=" + dt1;
            //var enc_str = HttpUtility.UrlEncode(Common.AES_Encrypt(str));  //10430675   AKIPP6385P


            ViewData["Message"] = "Success";
            if (!string.IsNullOrWhiteSpace(Request.QueryString.ToString()))
            {
                string key = Common.Create16DigitString();

                if (Session["Pass_Enc_Key"] == null && Session["Folio_No"] == null)
                {
                    Session["Pass_Enc_Key"] = key;
                    int _url_expire_days = Convert.ToInt32(ConfigurationManager.AppSettings["URLExpireDays"]);
                    DateTime dt = DateTime.Today;

                    var url = Request.Url.ToString();
                    TempData["URL"] = url;

                    var enc_folio = Request.QueryString.ToString();
                    enc_folio = HttpUtility.UrlDecode(enc_folio);
                    string decrypt_folio = Common.AES_Decrypt(enc_folio);

                    var _queryStringFDArray = decrypt_folio.Split('&').ToArray();
                    if (_queryStringFDArray != null && _queryStringFDArray.Length >= 2)
                    {
                        var _queryStringArrayDate = (_queryStringFDArray[1].ToString()).Split('=').ToArray();
                        if (_queryStringArrayDate != null && _queryStringArrayDate.Length >= 2)
                        {
                            DateTime _dt_url = Convert.ToDateTime(_queryStringArrayDate[1]);
                            int noOfDays = Convert.ToInt32((dt.Date - _dt_url.Date).TotalDays);

                            if (noOfDays >= 0 && noOfDays <= _url_expire_days)
                            {
                                string message = IBal.CheckLoginLog(url);
                                if (message == "Success")
                                {
                                    var _queryStringArrayFolio = (_queryStringFDArray[0].ToString()).Split('=').ToArray();
                                    if (_queryStringArrayFolio != null && _queryStringArrayFolio.Length >= 2)
                                    {
                                        var folioNo = _queryStringArrayFolio[1].ToString();
                                        TempData["Folio_No"] = folioNo;
                                    }
                                }
                                else
                                {
                                    ViewData["Message"] = "Error";
                                    if (message == "Block")
                                    {
                                        ViewData["ErrMessage"] = "Sorry! This link has been blocked. Try after some time...";
                                    }
                                    else if (message == "In-active")
                                    {
                                        ViewData["ErrMessage"] = "Sorry! This link is not active.";
                                    }
                                }
                            }
                            else
                            {
                                IBal.ExpireURL(url);
                                ViewData["Message"] = "Error";
                                ViewData["ErrMessage"] = "Sorry! This link has been expired.";
                            }
                        }
                    }
                }
                else
                {
                    if (Session["Pass_Enc_Key"] != null && Session["Folio_No"] == null)
                    {
                        var url = Request.Url.ToString();
                        TempData["URL"] = url;
                        return View();
                    }
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

        [HttpPost]
        [AjaxValidateAntiForgeryToken]
        public ActionResult AuthenticateUser(string HDPass)
        {
            string key = Session["Pass_Enc_Key"].ToString();
            //if (Request.IsAjaxRequest())
            //{
            //    Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
            //    RequestUtility _requestUtility = new RequestUtility();
            //    var DecryptedRequest = "";
            //    DecryptedRequest = AjaxRequestUtility.DecryptRequest(Request.InputStream, key);
            //    string[] RequestParams = new string[1] { "HDPass" };
            //    _requestUtility.GetRequestParamValues(DecryptedRequest, ref ParamDictionary, nameof(HDPass));
            //    HDPass = ParamDictionary[nameof(HDPass)];
            //}

            string Returnmsg = "";
            string err_msg = "";
            var url = TempData["URL"].ToString();
            string folioNo = TempData["Folio_No"].ToString();

            if (folioNo != null && Session["Pass_Enc_Key"] != null)
            {
                string pass = Common.DecryptStringAES(HDPass, Session["Pass_Enc_Key"].ToString());

                if (pass != "keyError")
                {
                    userD = IBal.AuthenticateUser(folioNo, pass);

                    if (userD.error == "" || userD.error == null)
                    {
                        if (userD.successflag == true)
                        {
                            err_msg = IBal.CheckLoginAttempts(url, true);

                            decimal invested_amt = 0, portfolio_val = 0, gain_loss = 0;
                            string colorCode = "#0095da";

                            invested_amt = Math.Round(Convert.ToDecimal(userD.lstSummaryTotal[0].totcostvalue), 2);
                            portfolio_val = Math.Round(Convert.ToDecimal(userD.lstSummaryTotal[0].totassets), 2);

                            if (invested_amt >= portfolio_val)
                            {
                                colorCode = "#ed5249";
                                gain_loss = (invested_amt - portfolio_val) * 100 / invested_amt;
                            }
                            else if (invested_amt < portfolio_val)
                            {
                                colorCode = "#4BB543";
                                gain_loss = (portfolio_val - invested_amt) * 100 / invested_amt;
                            }
                            else
                            {
                                colorCode = "#0095da";
                                gain_loss = 0;
                            }

                            Session["Folio_No"] = folioNo;
                            Session["Tot_Inv_Amt"] = invested_amt;
                            Session["Tot_Portfolio_Val"] = portfolio_val;
                            Session["Gain_Loss"] = Math.Round(gain_loss, 2);
                            Session["colorCode"] = colorCode;

                            Session["UserName"] = userD.userName;
                            Session["PAN_No"] = userD.PAN;
                            Session["Email"] = userD.email;

                            TempData["FundCatChartDt"] = userD.lstFundCat;
                            TempData["schTransDt"] = userD.lstFundCatTrans;

                            Returnmsg = "Success";
                        }
                        else if (userD.successflag == false)
                        {
                            err_msg = IBal.CheckLoginAttempts(url, false);
                            Returnmsg = err_msg;
                        }
                        else
                        {
                            Returnmsg = "Invalid Login.";
                        }
                    }
                    else
                    {
                        Returnmsg = "Error";
                    }
                }
                else
                {
                    Returnmsg = "Key error.";
                }
            }
            else
            {
                Returnmsg = "NoFolio";
            }
            Returnmsg = AjaxRequestUtility.EncryptStringAES(Returnmsg, key);
            return Json(new { ResponseBody = Returnmsg }, JsonRequestBehavior.AllowGet);
        }
    }
}