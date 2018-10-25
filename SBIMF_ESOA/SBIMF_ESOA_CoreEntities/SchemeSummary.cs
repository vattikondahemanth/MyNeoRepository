using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_CoreEntities
{
    public class SchemeSummary
    {
        public Address address { get; set; }
        public ReportScheme rptScheme { get; set; }        
        public List<Trans> schTrans { get; set; }
        public ReportPortfolioSummary rptSummary { get; set; }
    }
}
