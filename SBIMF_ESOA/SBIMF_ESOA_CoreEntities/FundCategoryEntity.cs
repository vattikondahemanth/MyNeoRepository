using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_CoreEntities
{
    public class FundCategoryEntity
    {
        public string CustomerName { get; set; }

        public string panNo { get; set; }

        public int FundCategoryId { get; set; }

        public string CategoryName { get; set; }

        public int Scheme_Count { get; set; }

        public decimal Invest_Amt { get; set; }

        public decimal Current_Value { get; set; }

        public decimal Scheme_Value { get; set; }

        public string gain_loss_tooltip { get; set; }

        public string SchemesList { get; set; }

        public List<ReportScheme> lst_Scheme { get; set; }
    }
}
