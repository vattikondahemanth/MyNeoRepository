using Newtonsoft.Json;
using SMSGateway.Interface;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static SMSGateway.DIFactory;

namespace SMSGateway
{
    public class SBIGateway : ISBIGateway, IGateWay
    {

        private string userid;
        private string password;
        private string mobileno;
        private string message;
        private string sendername;
        private string category;
        private string subject;
        private string smstype;
        private string clientmsgid;
        private readonly ICustomDependencyResolver resolver;
        public SBIGateway(ICustomDependencyResolver _resolver)
        {
            resolver = _resolver;
        }

        public async Task<SMSResponse> SendSMS(SMSData objSMS)
        {
            SMSResponse Response = resolver.Resolve<SMSResponse>();
            ILogger _logger = resolver.Resolve<ILogger>();
            var content = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                userid = Program.SBI_userid;
                password = Program.SBI_password;
                mobileno = objSMS.MobileNumber;
                message = objSMS.SMSMessage;
                sendername = Program.SBI_sendername;
                category = Program.SBI_category;
                subject = objSMS.Subject;
                smstype = Program.SBI_smstype;
                clientmsgid = Program.SBI_clientmsgid;
                var http = (HttpWebRequest)WebRequest.Create(new Uri(Program.SBI_url));
                http.ContentType = "application/json";
                http.Method = "POST";
                http.Timeout = 15000;
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] bytes = encoding.GetBytes(JsonConvert.SerializeObject(this));

                Stream newStream = http.GetRequestStreamAsync().Result;
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var response = await http.GetResponseAsync();
                var stream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
                
                Response.IsSMSSent = true;
                Response.MessageID = objSMS.MessageId;
            }
            catch (Exception e)
            {
                var _ExMessage = e.Message == null ? "Something Happened" : e.Message;
                var _InnerMessage = e.InnerException == null ? " " : e.InnerException.Message;
                var _Inner2Message = e.InnerException == null ? " " : (e.InnerException.InnerException == null ? " " : e.InnerException.InnerException.Message);
                _logger.InsertIntoSchedulerLog(1, "SendMessage", JsonConvert.SerializeObject(objSMS), Convert.ToString(_ExMessage + _InnerMessage + _Inner2Message), "SBISMSGateway");
                Response.IsSMSSent = false;
                Response.MessageID = objSMS.MessageId;

            }
            return Response;

        }

    }
}
