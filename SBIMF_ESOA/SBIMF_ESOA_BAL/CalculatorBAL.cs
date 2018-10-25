using SBIMF_ESOA_CoreEntities;
using SBIMF_ESOA_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SBIMF_ESOA_BAL
{
    public class CalculatorBAL
    {
        private static IGenericDAL<Schemes> _obj;
        InvestorPANDetails panDetails= new InvestorPANDetails();
        ErrorLogBAL errLogBAL = new ErrorLogBAL();

        public CalculatorBAL() : this(new GenericDAL<Schemes>())
        {

        }

        public CalculatorBAL(GenericDAL<Schemes> on)
        {
            _obj = on;
        }

        public List<Schemes> GetSchemes()
        {
            List<Schemes> sch = new List<Schemes>();
            try
            {
                string[] parameter = new string[] { };
                string jsonResponse = _obj._website_serviceObject.SBIMFJSONService("usp_GetSchemeForSWPCalculation", parameter);
                DataSet ds = new DataSet();
                ds = JsonConvert.DeserializeObject<DataSet>(jsonResponse);
                sch = Common.GetResultType<List<Schemes>>(ds.Tables[0]);
            }
            catch (Exception ex)
            {
                errLogBAL.InsertErroLog("GetSchemes", "Calculator", ex.Message, ex.StackTrace);
            }
            return sch;
        }

        public InvestorPANDetails CheckKycForInvestorInfo(string pan)
        {
            string ExecutionContext = Guid.NewGuid().ToString();
            try
            {
                var kraDetails = _obj._serviceObject.VerifyPANDetailsBasedonConfigSettings("", pan, "01-06-1999", "");
                if (kraDetails.ReturnCode == "0")
                {
                    DataSet ds = new DataSet();                    
                    panDetails.IsKYC = kraDetails.Data.IsKYC;
                }
             }
            catch (Exception ex)
            {
                errLogBAL.InsertErroLog("CheckKycForInvestorInfo", "Calculator", ex.Message, ex.StackTrace);
            }
            return panDetails;
        }

        public string SBIMFJSONService(string MethodName, string[] parameters)
        {
            string SBIMFJSONServiceResult = string.Empty;
            try
            { 
                SBIMFJSONServiceResult = _obj._website_serviceObject.SBIMFJSONService(MethodName, parameters);
            }
            catch (Exception ex)
            {
                errLogBAL.InsertErroLog("SBIMFJSONService", "Calculator", ex.Message, ex.StackTrace);
            }
            return SBIMFJSONServiceResult;
        }

    }
}
