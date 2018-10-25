using SBIMF_ESOA_CoreEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBIMF_ESOA_WebApp.Controllers
{
    public class ErrorController : Controller
    {      
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetError(string controller_name, string action_name, string err_msg, string stack_trace)
        {
            ErrModel errModel = new ErrModel();
            errModel.controllerName = controller_name;
            errModel.actionName = action_name;
            errModel.err_msg = err_msg;
            errModel.stack_trace = stack_trace;

            TempData["ErrData"] = errModel;

            return Json(errModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplayError()
        {
            //ErrModel errModel = new ErrModel();
            //errModel = (ErrModel)TempData.Peek("ErrData");
            return View();
        }
    }
}