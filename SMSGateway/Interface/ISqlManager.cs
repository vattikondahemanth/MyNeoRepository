using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Interface
{
    public interface ISqlManager
    {
        SqlConnection GetSqlConnection { get; }
        bool CloseSqlConnection { get; }
    }
}
