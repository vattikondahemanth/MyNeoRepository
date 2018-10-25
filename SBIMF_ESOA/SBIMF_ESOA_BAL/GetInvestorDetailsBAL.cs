using SBIMF_ESOA_CoreEntities;
using SBIMF_ESOA_DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using System.IO;

namespace SBIMF_ESOA_BAL
{
    public class GetInvestorDetailsBAL
    {
        private static IGenericDAL<InvestorInfo> _obj;
        public GetInvestorDetailsBAL() : this(new GenericDAL<InvestorInfo> ()) 
        {

        }

        public GetInvestorDetailsBAL(GenericDAL<InvestorInfo> on)
        {
            _obj = on;
        }

        public InvestorInfo GetFolioDetatils(string folioNo)
        {
            InvestorInfo invInfo = new InvestorInfo();
            string obj_Invinfo = _obj._serviceObject.Investor_Information(folioNo,"BUQPS3847C").ToString();
            if (!string.IsNullOrEmpty(obj_Invinfo))
            {
                var tables = JsonConvert.DeserializeObject<DataSet>(obj_Invinfo);
                if (tables != null && tables.Tables.Count > 0)
                {
                    var scheme_tbl = JsonConvert.SerializeObject(tables.Tables[4]);

                    //Get scheme details in Scheme Entity
                    List<SchemeEntity> lst_Scheme = Common.JsonToClass<List<SchemeEntity>>(JsonConvert.SerializeObject(tables.Tables[4]));
                    //var scheme_codes = from s in lst_Scheme select s.Scheme_code;
                    string[] scheme_codes = lst_Scheme.Select(x => x.Scheme_code).OrderBy(x => x).ToArray();
                }
                return Common.JsonToClass<List<InvestorInfo>>(JsonConvert.SerializeObject(tables.Tables[1])).FirstOrDefault();
            }
            return invInfo;
        }
        
        public InvestorInfo PopulateRecord(SqlDataReader reader)
        {
            return new InvestorInfo
            {
                Investor_Name = reader.GetString(0),
                Email = reader.GetString(1),
                mobileno = reader.GetString(2)
            };
        }
    }
}
