using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_CoreEntities
{
    public class AccountStatement
    {
        public List<Address> lst_Address { get; set; }
        public List<Trans> lst_Trans { get; set; }
        public List<ReportScheme> lst_ReportScheme { get; set; }
        public List<ReportPortfolioSummary> lst_ReportPortfolioSummary { get; set; }
    }
}
