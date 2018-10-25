using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_CoreEntities
{
    public class FundCatTransactions
    {
        public List<FundCategoryEntity> fundCatEntity {get; set;}

        public List<Trans> lstTrans { get; set; }

        public List<Address> address { get; set; }

        public List<ReportScheme> lstSchemes { get; set; }

        public List<ReportPortfolioSummary> lstPortfolioSummary { get; set; }

        public List<SummaryTotal> lstSummaryTotal { get; set; }
    }
}
