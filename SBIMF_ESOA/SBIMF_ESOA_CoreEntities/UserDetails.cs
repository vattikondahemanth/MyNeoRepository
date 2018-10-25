using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_CoreEntities
{
    public class UserDetails
    {
        public string userName { get; set; }

        public string HDPass { get; set; }

        public string HDKey { get; set; }

        public string folioNo { get; set; }

        public string PAN { get; set; }

        public string email { get; set; }

        public string phoneNo { get; set; }

        public bool successflag { get; set; }

        public string error { get; set; }

        public List<SummaryTotal> lstSummaryTotal { get; set; }

        public List<FundCategoryEntity> lstFundCat { get; set; }

        public List<Trans> lstFundCatTrans { get; set; }

    }
}
