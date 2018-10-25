using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_CoreEntities
{
    public class SchemeEntity
    {
        public int SchemeId { get; set; }
        public string Scheme_code { get; set; }
        public string Scheme { get; set; }
        public int FundId { get; set; }
        public string FundName { get; set; }
        public int FundCategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
