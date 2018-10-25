using SBIMF_ESOA_CoreEntities;
using SBIMF_ESOA_DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_BAL
{
    public class SchTransactionBAL
    {
        private static IGenericDAL<Trans> _obj;
        public SchTransactionBAL() : this(new GenericDAL<Trans> ()) 
        {

        }

        public SchTransactionBAL(GenericDAL<Trans> on)
        {
            _obj = on;
        }
    }
}
