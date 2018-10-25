using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_CoreEntities
{
    public class InvestorInfo
    {
        public string FolioNo { get; set; }
        
        public string Investor_Name { get; set; }
        
        public string Email { get; set; }
        
        public string mobileno { get; set; }
        
        public string PAN { get; set; }

    }
}
