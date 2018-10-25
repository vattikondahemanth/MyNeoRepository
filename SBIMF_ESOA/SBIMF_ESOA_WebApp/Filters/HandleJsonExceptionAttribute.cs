using SBIMF_ESOA_BAL;
using SBIMF_ESOA_CoreEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBIMF_ESOA_WebApp.Filters
{
    public class HandleErrorAttribute: ActionFilterAttribute
    {
        private ErrorLogBAL IBal;
        ErrModel err = new ErrModel();

        public HandleErrorAttribute(ErrorLogBAL obj)
        {
            this.IBal = obj;
        }

        public HandleErrorAttribute() : this(new ErrorLogBAL())
        { }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string actionName= String.Empty, controllerName = String.Empty, msg = String.Empty, stackTrace = String.Empty;
            if (filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception != null)
            {
                actionName = filterContext.ActionDescriptor.ActionName.ToString();
                controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToString();
                msg = filterContext.Exception.Message.ToString();
                stackTrace = filterContext.Exception.StackTrace.ToString();

                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                filterContext.Result = new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        filterContext.ActionDescriptor.ActionName,
                        filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                        filterContext.Exception.Message,
                        filterContext.Exception.StackTrace
                    }
                };
                IBal.InsertErroLog(actionName, controllerName, msg, stackTrace);
                filterContext.ExceptionHandled = true;
            }
            else if(filterContext.Exception != null && (!filterContext.ExceptionHandled))
            {
                actionName = filterContext.ActionDescriptor.ActionName;
                controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                msg = filterContext.Exception.Message;
                stackTrace = filterContext.Exception.StackTrace;
                IBal.InsertErroLog(actionName, controllerName, msg, stackTrace);
                filterContext.ExceptionHandled = true;         
                filterContext.Result = new ViewResult{ ViewName = "~/Views/Shared/Error.cshtml"  };
            }

        }
        
    }
}