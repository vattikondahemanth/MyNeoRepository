using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_CoreEntities
{
    public class InvestorPANDetails
    {
        public string userid { get; set; }
        public string PanNumber { get; set; }
        public string DOB { get; set; }
        public string InvestorName { get; set; }
        public bool IsKYC { get; set; }
        public string APP_STATUSDT { get; set; }
        public string APP_ENTRYDT { get; set; }
        public string APP_MODDT { get; set; }
        public string APP_UPDT_STATUS { get; set; }
        public string APP_HOLD_DEACTIVE_RMKS { get; set; }
        public string CAMSKRA { get; set; }
        public string CVLKRA { get; set; }
        public string NDMLKRA { get; set; }
        public string KARVYKRA { get; set; }
        public string DOTEXKRA { get; set; }
        public string APP_STATUS { get; set; }
        public bool isCKY { get; set; }
        public string APP_KYC_MODE { get; set; }
        public string KRAServiceProvider { get; set; }
        public string APP_IPV_FLAG { get; set; }
    }
}
