using SMSGateway.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway
{
    public interface ISMSSender
    {
        IGateWay GetSMSProvider(SMSProviders sMSProviders);
        Task<bool> InsertSentItemAsync(int messageId, string mobileNo, string smsMessage, string subject, DateTime transDatetime, string status, string priority);
        
        IList<SMSData> GetOutboxMessageAsync();
    }
}
