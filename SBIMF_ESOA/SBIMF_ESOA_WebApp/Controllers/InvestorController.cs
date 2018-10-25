using SBIMF_ESOA_BAL;
using SBIMF_ESOA_CoreEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBIMF_ESOA_WebApp.Controllers
{
    public class InvestorController : Controller
    {
        private GetInvestorDetailsBAL IBal;
   
        public InvestorController(GetInvestorDetailsBAL obj)
        {
            this.IBal = obj;
        }

        public InvestorController(): this(new GetInvestorDetailsBAL())
        { }
        // GET: Investor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetInvestorDetails()  //string FolioNo
        {
            string FolioNo = "13627577";
            InvestorInfo InvInfo = IBal.GetFolioDetatils(FolioNo);
            return View(InvInfo);
        }
    }
}