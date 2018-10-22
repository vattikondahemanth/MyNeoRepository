using SMSGateway.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway
{
    public class SMSResponse : ISMSResponse
    {
        public int MessageID;
        public bool IsSMSSent;
    }
}
