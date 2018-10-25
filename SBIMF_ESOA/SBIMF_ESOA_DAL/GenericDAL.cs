using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBIMF_ESOA_DAL.SBIMFSERVICE;
//using AutoMapper;
//using Dapper;
using System.Reflection;
using SBIMF_ESOA_DAL.SBIMFWebsiteSSL;

namespace SBIMF_ESOA_DAL
{
    public class GenericDAL<T> : IGenericDAL<T> where T : class
    {

        private static string _sqlConstring;
        private IDbConnection _dbConn;
        private static string _shareplexConstring;
        private static SqlConnection _connection;
        public static SBIMFSERVICE.SBIMFSERVICEClient _serviceObject;
        public static SBIMFWebsiteSSL.SBIMF_WebSiteServiceClient _website_serviceObject;

        SBIMFSERVICEClient IGenericDAL<T>._serviceObject
        {
            get
            {
                return _serviceObject = new SBIMFSERVICE.SBIMFSERVICEClient();
            }
        }
        SBIMF_WebSiteServiceClient IGenericDAL<T>._website_serviceObject
        {
            get
            {
                return _website_serviceObject = new SBIMFWebsiteSSL.SBIMF_WebSiteServiceClient();
            }
        }

        public GenericDAL()
        {
            _sqlConstring = ConfigurationManager.ConnectionStrings["SQL"].ToString();
            _shareplexConstring = ConfigurationManager.ConnectionStrings["SharePlex"].ToString();
            _connection = new SqlConnection(_sqlConstring);
            _dbConn = _connection;
            // _serviceObject = new SBIMFSERVICE.SBIMFSERVICEClient();
        }


        public virtual T PopulateRecord(SqlDataReader reader)
        {
            return null;
        }
        public IEnumerable<T> GetRecords(SqlCommand command, bool IsSQL = true)
        {
            if (!IsSQL)
            {
                _connection = new SqlConnection(_shareplexConstring);
            }

            var list = new List<T>();
            command.Connection = _connection;
            _connection.Open();
            try
            {
                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                        list.Add(PopulateRecord(reader));
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                _connection.Close();
            }
            return list;
        }

        public T GetRecord(SqlCommand command, bool IsSQL = true)
        {
            if (!IsSQL)
            {
                _connection = new SqlConnection(_shareplexConstring);
            }
            T record = null;
            command.Connection = _connection;
            _connection.Open();
            try
            {
                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        record = PopulateRecord(reader);
                        break;
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                _connection.Close();
            }
            return record;
        }

        private static PropertyInfo GetTargetProperty(string name)
        {

            return typeof(T).GetProperties().Where(x => x.Name == name).ToList().First();
            //return typeof(T).GetProperties()
            //                .Where(p => p.GetCustomAttributes(typeof(DataBind), true)
            //                    .Where(a => ((DataBind)a).ColumnName == name)
            //                    .Any()
            //                    ).FirstOrDefault();
        }

        public int InsertData(SqlCommand command, bool IsSQL = true)
        {
            int i = 0;
            if (!IsSQL)
            {
                _connection = new SqlConnection(_shareplexConstring);
            }            
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;
            _connection.Open();
            try
            {
                i = command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }
            return i;
        }


        public DataSet GetDataSetExecuteStoredProc(SqlCommand command, bool IsSQL = true)
        {
            DataSet ds = new DataSet();
            if (!IsSQL)
            {
                _connection = new SqlConnection(_shareplexConstring);
            }            
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;

            _connection.Open();
            try
            {

                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds);
            }
            finally
            {
                _connection.Close();
            }
            return ds;
        }
        public IEnumerable<T> ExecuteStoredProc<T>(SqlCommand command, bool IsSQL = true)
        {

            if (!IsSQL)
            {
                _connection = new SqlConnection(_shareplexConstring);
            }

            var list = new List<T>();
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;
            _connection.Open();
            try
            {
                var dataReader = command.ExecuteReader();
                try
                {
                    //while (dataReader.Read())
                    //{
                    //    //var record = PopulateRecord(reader);
                    //    //if (record != null) list.Add(record);

                    //    if (dataReader.HasRows)
                    //        return Mapper.Map<IDataReader, IEnumerable<T>>(dataReader);
                    //}
                    while (dataReader.Read())
                    {
                        var item = Activator.CreateInstance(typeof(T));
                        for (int columnIndex = 0; columnIndex < dataReader.FieldCount; columnIndex++)
                        {
                            var objectProperty = GetTargetProperty(dataReader.GetName(columnIndex));
                            if (objectProperty != null)
                            {
                                var dataValue = dataReader.GetValue(columnIndex);
                                if (objectProperty.PropertyType == typeof(List<int>))
                                {
                                    objectProperty.SetValue(item, DBNull.Value.Equals(dataValue) ? null : dataValue.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i.Trim())).ToList<int>());
                                }
                                else
                                {
                                    objectProperty.SetValue(item, DBNull.Value.Equals(dataValue) ? null : dataValue);
                                }
                            }
                        }

                        yield return (T)item;
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    dataReader.Close();
                }
            }
            finally
            {
                _connection.Close();
            }
            //return list;
        }

    }
}
