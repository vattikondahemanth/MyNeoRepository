using SBIMF_ESOA_WebApp.Filters;
using System.Web;
using System.Web.Mvc;

namespace SBIMF_ESOA_WebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new Filters.HandleErrorAttribute());
        }
    }
}
