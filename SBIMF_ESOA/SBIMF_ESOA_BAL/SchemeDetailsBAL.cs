using Newtonsoft.Json;
using SBIMF_ESOA_CoreEntities;
using SBIMF_ESOA_DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace SBIMF_ESOA_BAL
{
    public class SchemeDetailsBAL
    {
        AccessSession _objSession = new AccessSession();
        private static IGenericDAL<SchemeEntity> _obj;
        public SchemeDetailsBAL() : this(new GenericDAL<SchemeEntity> ()) 
        {

        }

        public SchemeDetailsBAL(GenericDAL<SchemeEntity> on)
        {
            _obj = on;
        }

        public List<SchemeEntity> GetAllSchemesByFundCategory(int FundCatId,string schemes=null)
        {
            List<SchemeEntity> lstSchemes;
            
            SqlCommand cmd = new SqlCommand("Usp_getAllSchemesByFundCat");
            cmd.Parameters.AddWithValue("@FundCategoryId", FundCatId);
            cmd.Parameters.AddWithValue("@Scheme_codes", schemes);
            lstSchemes = _obj.ExecuteStoredProc<SchemeEntity>(cmd).ToList();

            if (lstSchemes.Count > 0)
                return lstSchemes;
            else
                throw new Exception();
        }
    }
}
