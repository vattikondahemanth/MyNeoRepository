using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace SBIMF_ESOA_BAL
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class AccessSession : IRequiresSessionState
    {
        public AccessSession()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        public void SetSession(string key, object value)
        {
            HttpContext.Current.Session.Add(key, value);
        }

        public object GetSession(string key)
        {

            object sessionKey = HttpContext.Current.Session[key];
            return sessionKey;
        }
    }
}
