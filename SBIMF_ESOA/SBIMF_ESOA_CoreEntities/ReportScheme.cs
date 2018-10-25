using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_CoreEntities
{
    public class ReportScheme
    {
        
        public string No { get; set; }
        
        public string Code { get; set; }

        public string amccode { get; set; }

        public string Lname { get; set; }

        public string Name { get; set; }
        
        public string costvalue { get; set; }

        public string Opbal { get; set; }
        
        public string Nav { get; set; }
        
        public string Schremark { get; set; }
        
        public string ISIN { get; set; }
        
        public string brokername { get; set; }
        
        public string bankacct { get; set; }
        
        public string lien_unit { get; set; }

        public string markvalue { get; set; }

        public decimal currentvalue { get; set; }

        public decimal gain_loss_value { get; set; }

        public string gain_loss { get; set; }

        public int FundCategoryId { get; set; }

        public string CategoryName { get; set; }

        //public List<Trans> schemetransact { get; set; }

        //public string Baldate { get; set; }
    }
}
