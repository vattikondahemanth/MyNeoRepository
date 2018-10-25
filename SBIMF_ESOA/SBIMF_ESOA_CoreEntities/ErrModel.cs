using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBIMF_ESOA_CoreEntities
{
    public class ErrModel
    {
        public string controllerName { get; set; }

        public string actionName { get; set; }

        public string err_msg { get; set; }

        public string stack_trace { get; set; }
    }
}
