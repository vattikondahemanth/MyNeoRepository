
using SBIMF_ESOA_CoreEntities;
using SBIMF_ESOA_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_BAL
{
    public class ErrorLogBAL
    {

        private static IGenericDAL<ErrModel> _obj;

        public ErrorLogBAL() : this(new GenericDAL<ErrModel>())
        {

        }

        public ErrorLogBAL(GenericDAL<ErrModel> on)
        {
            _obj = on;
        }

        public void InsertErroLog(string actionName, string controllerName, string msg, string stackTrace)
        {
            SqlCommand cmd = new SqlCommand("usp_CreateESOAErrorLog");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@actionDesc", controllerName + "/" + actionName);
            cmd.Parameters.AddWithValue("@errorDesc", msg);
            cmd.Parameters.AddWithValue("@stackTrace", stackTrace);

            int i = _obj.InsertData(cmd);
        }
    }
}
