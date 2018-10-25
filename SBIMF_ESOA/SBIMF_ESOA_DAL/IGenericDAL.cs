using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SBIMF_ESOA_DAL
{
    public interface IGenericDAL<T> where T : class
    {

        SBIMFSERVICE.SBIMFSERVICEClient _serviceObject { get; }
        SBIMFWebsiteSSL.SBIMF_WebSiteServiceClient _website_serviceObject { get; }
        T PopulateRecord(SqlDataReader reader);
        IEnumerable<T> GetRecords(SqlCommand command, bool IsSQL = true);
        T GetRecord(SqlCommand command, bool IsSQL = true);
        DataSet GetDataSetExecuteStoredProc(SqlCommand command, bool IsSQL = true);
        IEnumerable<T> ExecuteStoredProc<T>(SqlCommand command, bool IsSQL = true);
        int InsertData(SqlCommand command, bool IsSQL = true);
    }
}
