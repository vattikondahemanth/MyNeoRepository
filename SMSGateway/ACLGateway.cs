using Newtonsoft.Json;
using SMSGateway.Interface;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static SMSGateway.DIFactory;

namespace SMSGateway
{
    public class ACLGateway : IACLGateway, IGateWay
    {
        private readonly ICustomDependencyResolver resolver;
        public ACLGateway(ICustomDependencyResolver _resolver)
        {
            resolver = _resolver;
        }
        async Task<SMSResponse> IGateWay.SendSMS(SMSData objSMS)
        {
            SMSResponse Response = resolver.Resolve<SMSResponse>();
            ILogger _logger = resolver.Resolve<ILogger>();
            try
            {
                string dataString = string.Empty;
                string ACLURL = ConfigurationManager.AppSettings["NFTSMSURL"].ToString();
                string finalURL = ACLURL.Replace("[XXXMobileNumberXXX]", objSMS.MobileNumber).Replace("[XXXMessageXXX]", objSMS.SMSMessage).Replace("+", "");
                WebRequest request = HttpWebRequest.Create(finalURL);
                request.Timeout = 15000;
                HttpWebResponse httpResponse = (HttpWebResponse)await request.GetResponseAsync();
                Stream s = (Stream)httpResponse.GetResponseStream();
                using (StreamReader reader = new StreamReader(s))
                {
                    dataString = reader.ReadToEnd();
                }
                Response.IsSMSSent = true;
                Response.MessageID = objSMS.MessageId;
            }
            catch (Exception e)
            {
                var _ExMessage = e.Message == null ? "Something Happened" : e.Message;
                var _InnerMessage = e.InnerException == null ? " " : e.InnerException.Message;
                var _Inner2Message = e.InnerException == null ? " " : (e.InnerException.InnerException == null ? " " : e.InnerException.InnerException.Message);
                _logger.InsertIntoSchedulerLog(1, "SendMessage", JsonConvert.SerializeObject(objSMS), Convert.ToString(_ExMessage + _InnerMessage +  _Inner2Message), "ACLSMSGateway");
                Response.IsSMSSent = false;
                Response.MessageID = objSMS.MessageId;
            }
            
            return Response;
        }
    }
}
