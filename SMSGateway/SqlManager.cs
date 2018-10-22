using SMSGateway.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway
{
    public class SqlManager : ISqlManager
    {
        private SqlConnection con;
        private readonly Object obj = new object();
        public SqlConnection GetSqlConnection {
            get
            {
                if (con == null)
                {

                    lock (obj)
                    {
                        if (con == null)
                        {
                            con = new SqlConnection(Program.connectionString);
                            con.Open();
                            return con;
                        }
                    }
                }
                return con;
            }
           
        }

        public bool CloseSqlConnection {
            get {

                if (con.State != ConnectionState.Closed)
                {

                    lock (obj)
                    {
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Close();
                            con.Dispose();
                            return true;
                        }
                    }
                }
                return true;
            }
        }
    }
}
