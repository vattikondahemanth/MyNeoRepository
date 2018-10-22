using SMSGateway.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway
{
    public class SMSData : ISMSData
    {
        public int MessageId { get; set; }
        public string MobileNumber { get; set; }
        public string TransDatetime { get; set; }
        public string SMSMessage { get; set; }
        public string SubmitDatetime { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int ServiceProvider { get; set; }
    }
}
