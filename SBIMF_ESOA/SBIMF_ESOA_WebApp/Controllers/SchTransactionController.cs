using SBIMF_ESOA_BAL;
using SBIMF_ESOA_CoreEntities;
using SBIMF_ESOA_WebApp.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBIMF_ESOA_WebApp.Controllers
{
    [SessionTimeout]
    //[HandleJsonException]
    public class SchTransactionController : Controller
    {
        // GET: SchTransaction
        private SchTransactionBAL IBal;

        public SchTransactionController(SchTransactionBAL obj)
        {
            this.IBal = obj;
        }

        public SchTransactionController(): this(new SchTransactionBAL())
        { }
        // GET: Scheme
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTransactionsfromChart(List<Trans> schTransactions)
        {
            TempData["schTransactions"] = schTransactions;
            return Json(schTransactions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplayTransactionsfromChart()
        {
            List<Trans> schTransactions = (List<Trans>)(TempData.Peek("schTransactions"));
            return View(schTransactions);
        }
    }
}