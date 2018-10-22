using SMSGateway.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SMSGateway.DIFactory;

namespace SMSGateway
{
    public class Logger : ILogger
    {
        private readonly ICustomDependencyResolver resolver;
        public Logger(ICustomDependencyResolver _resolver)
        {
            resolver = _resolver;
        }
        public void InsertIntoSchedulerLog(int logType, string methodName, string request, string response, string schedulerName)
        {
            
            SqlConnection con = Program.SqlManager.GetSqlConnection;
            SqlCommand cmd = resolver.Resolve<SqlCommand>();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_web_InsertIntoSchedulerlog";
            cmd.Parameters.Add("@LogType", SqlDbType.Int).Value = logType;
            cmd.Parameters.Add("@MethodName", SqlDbType.VarChar).Value = methodName;
            cmd.Parameters.Add("@Request", SqlDbType.VarChar).Value = request;
            cmd.Parameters.Add("@Response", SqlDbType.VarChar).Value = response;
            cmd.Parameters.Add("@SchedulerName", SqlDbType.VarChar).Value = schedulerName;
            cmd.Connection = con;
            cmd.ExecuteNonQuery();
        }
    }
}
