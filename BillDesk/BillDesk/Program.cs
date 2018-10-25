using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BillDesk
{
    class Program
    {
        static void Main(string[] args)
        {
            REQUEST r = new REQUEST();
            r.CHECKSUM = "919F6F7D2C3D644A4A09D4735B0F5F5E4DB2B0BC4954B8737A4994E3CF4AFD86";
            r.TXNDATA = new TXNDATA();
            r.TXNDATA.TXNSUMMARY = new TXNSUMMARY();
            r.TXNDATA.TXNSUMMARY.PGCUSTOMERID = "121111";
            r.TXNDATA.TXNSUMMARY.PGMERCID = "SBIMFECOM";
            r.TXNDATA.TXNSUMMARY.RECORDS = "1";
            r.TXNDATA.TXNSUMMARY.REQID = "PGECOM201";
            r.TXNDATA.TXNSUMMARY.TXNDATE = "20181023172659";
            r.TXNDATA.TXNSUMMARY.AMOUNT = "400.00";

            var rec = new RECORD();
            rec.CUSTOMERID = "9920090";
            rec.ADDITIONALINFO1 = "NA";
            rec.ADDITIONALINFO2 = "NA";
            rec.ADDITIONALINFO3 = "NA";
            rec.ADDITIONALINFO4 = "NA";
            rec.ADDITIONALINFO5= "NA";
            rec.ADDITIONALINFO6 = "NA";
            rec.ADDITIONALINFO7 = "NA";
            rec.AMOUNT = "400.00";
            rec.FILLER1 = "NA";
            rec.FILLER2 = "NA";
            rec.FILLER3 = "NA";
            rec.ID = "1";
            rec.MERCID = "SBIMFSCHM1";
            r.TXNDATA.RECORD = new List<RECORD>();
            r.TXNDATA.RECORD.Add(rec);

            string xmldata = r.Serialize();
            Program p = new Program();
            const string checksumKey = "n3K2HVvpVzAL";
            string req1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><REQUEST>";
            string req = "<TXNDATA><TXNSUMMARY><REQID>PGECOM201</REQID><PGMERCID>SBIMFECOM</PGMERCID><RECORDS>2</RECORDS><PGCUSTOMERID>96998956634</PGCUSTOMERID><AMOUNT>400.00</AMOUNT><TXNDATE>" + DateTime.Now.ToString("yyyyMMddHHmmss") + "</TXNDATE></TXNSUMMARY>";
            req += "<RECORD ID=\"1\"><MERCID>SBIMFSCHM1</MERCID><AMOUNT>200.00</AMOUNT><CUSTOMERID>99200998485</CUSTOMERID>" +
                      "<ADDITIONALINFO1>9920098485</ADDITIONALINFO1>" +
                      "<ADDITIONALINFO2>13761476</ADDITIONALINFO2>" +
                      "<ADDITIONALINFO3>DIRECT</ADDITIONALINFO3>" +
                      "<ADDITIONALINFO4>EQUITY</ADDITIONALINFO4>" +
                      "<ADDITIONALINFO5>D512G</ADDITIONALINFO5>" +
                      "<ADDITIONALINFO6>Tejesh</ADDITIONALINFO6>" +
                      "<ADDITIONALINFO7>1-2</ADDITIONALINFO7>" +
                      "<FILLER1>NA</FILLER1>" +
                      "<FILLER2>NA</FILLER2>" +
                      "<FILLER3>NA</FILLER3>" +
                      "</RECORD>";
            req += "<RECORD ID=\"2\"><MERCID>SBIMFSCHM2</MERCID><AMOUNT>200.00</AMOUNT><CUSTOMERID>99209181054</CUSTOMERID>" +
                     "<ADDITIONALINFO1>992854</ADDITIONALINFO1>" +
                     "<ADDITIONALINFO2>13761476</ADDITIONALINFO2>" +
                     "<ADDITIONALINFO3>DIRECT</ADDITIONALINFO3>" +
                     "<ADDITIONALINFO4>EQUITY</ADDITIONALINFO4>" +
                     "<ADDITIONALINFO5>D513G</ADDITIONALINFO5>" +
                     "<ADDITIONALINFO6>Tejesh</ADDITIONALINFO6>" +
                     "<ADDITIONALINFO7>2-2</ADDITIONALINFO7>" +
                     "<FILLER1>NA</FILLER1>" +
                     "<FILLER2>NA</FILLER2>" +
                     "<FILLER3>NA</FILLER3>" +
                     "</RECORD>";
            req += "</TXNDATA>";
            req1 += req;
            string checksumValue = p.GetHMACSHA256(req, checksumKey).ToUpper();
            req1 += "<CHECKSUM>" + checksumValue + "</CHECKSUM></REQUEST>";
            string response = p.PostReq("https://payments.billdesk.com/ecom/ECOM2ReqHandler", "msg=" + req1);
            if (!string.IsNullOrWhiteSpace(response))
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(response);
                var nodes = xml.SelectNodes("/RESPONSE");
                string requestForCheckSum = nodes[0].ChildNodes[0].OuterXml.ToString();
                string responseCheckSum = nodes[0].ChildNodes[1].InnerText;
                string chksumVal = p.GetHMACSHA256(requestForCheckSum, checksumKey).ToUpper();
                if (chksumVal == responseCheckSum)
                {
                  
                }
            }
        }
        public string GetHMACSHA256(string text, string key)
        {
            UTF8Encoding encoder = new UTF8Encoding();

            byte[] hashValue;
            byte[] keybyt = encoder.GetBytes(key);
            byte[] message = encoder.GetBytes(text);

            HMACSHA256 hashString = new HMACSHA256(keybyt);
            string hex = "";

            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
        private string PostReq(string URL, string Request)
        {
            //var result = new ServiceResult();
            string a = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            //request.Proxy.Credentials = (NetworkCredential)CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(Request);
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                a = reader.ReadToEnd();
                //result.ReturnCode = 0;
                //result.ReturnMsg = "Success";

                stream.Dispose();
                reader.Dispose();

            }
            catch (WebException ex1)
            {
                //result.ReturnCode = 400;
                // result.ReturnMsg = "Billdesk : " + ex1.Message;
            }
            catch (Exception ex)
            {
                // result.ReturnCode = 201;
                // result.ReturnMsg = ex.Message;
            }
            return a;
        }
        private string SendRequest(string URL, string Request)
        {
            var content = string.Empty;
            try
            {
                var http = (HttpWebRequest)WebRequest.Create(new Uri(URL));
                http.ContentType = "application/x-www-form-urlencoded";
                http.Method = "POST";
                //http.Proxy = new WebProxy();
                //IDictionaryEnumerator headers = getHeaders().GetEnumerator();
                //while (headers.MoveNext())
                //{
                //    http.Headers.Add(headers.Key.ToString(), headers.Value.ToString());
                //}
                string requestBody = Request;// CreateRequest(Request);
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(requestBody);

                Stream newStream = http.GetRequestStreamAsync().Result;
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = http.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                //content = Verifyresponse(sr.ReadToEnd());
                content = sr.ReadToEnd();
                //Response.Write(content);
            }
            catch (WebException wex)
            {
                //ObjSbi.WebSiteExceptionLog("SendRequest", "Request:" + Request + ", URL:" + URL, wex.Message, "", 999);
            }
            catch (Exception ex)
            {
                //ObjSbi.WebSiteExceptionLog("SendRequest", "Request:" + Request + ", URL:" + URL, ex.Message, "", 999);
            }
            return content;
        }

    }
    public static class MyClass
    {
        public static string Serialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
    }
    [XmlRoot(ElementName = "TXNSUMMARY")]
    public class TXNSUMMARY
    {
        [XmlElement(ElementName = "REQID")]
        public string REQID { get; set; }
        [XmlElement(ElementName = "PGMERCID")]
        public string PGMERCID { get; set; }
        [XmlElement(ElementName = "RECORDS")]
        public string RECORDS { get; set; }
        [XmlElement(ElementName = "PGCUSTOMERID")]
        public string PGCUSTOMERID { get; set; }
        [XmlElement(ElementName = "AMOUNT")]
        public string AMOUNT { get; set; }
        [XmlElement(ElementName = "TXNDATE")]
        public string TXNDATE { get; set; }
    }

    [XmlRoot(ElementName = "RECORD")]
    public class RECORD
    {
        [XmlElement(ElementName = "MERCID")]
        public string MERCID { get; set; }
        [XmlElement(ElementName = "AMOUNT")]
        public string AMOUNT { get; set; }
        [XmlElement(ElementName = "CUSTOMERID")]
        public string CUSTOMERID { get; set; }
        [XmlElement(ElementName = "ADDITIONALINFO1")]
        public string ADDITIONALINFO1 { get; set; }
        [XmlElement(ElementName = "ADDITIONALINFO2")]
        public string ADDITIONALINFO2 { get; set; }
        [XmlElement(ElementName = "ADDITIONALINFO3")]
        public string ADDITIONALINFO3 { get; set; }
        [XmlElement(ElementName = "ADDITIONALINFO4")]
        public string ADDITIONALINFO4 { get; set; }
        [XmlElement(ElementName = "ADDITIONALINFO5")]
        public string ADDITIONALINFO5 { get; set; }
        [XmlElement(ElementName = "ADDITIONALINFO6")]
        public string ADDITIONALINFO6 { get; set; }
        [XmlElement(ElementName = "ADDITIONALINFO7")]
        public string ADDITIONALINFO7 { get; set; }
        [XmlElement(ElementName = "FILLER1")]
        public string FILLER1 { get; set; }
        [XmlElement(ElementName = "FILLER2")]
        public string FILLER2 { get; set; }
        [XmlElement(ElementName = "FILLER3")]
        public string FILLER3 { get; set; }
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "TXNDATA")]
    public class TXNDATA
    {
        [XmlElement(ElementName = "TXNSUMMARY")]
        public TXNSUMMARY TXNSUMMARY { get; set; }
        [XmlElement(ElementName = "RECORD")]
        public List<RECORD> RECORD { get; set; }
    }

    [XmlRoot(ElementName = "REQUEST")]
    public class REQUEST
    {
        [XmlElement(ElementName = "TXNDATA")]
        public TXNDATA TXNDATA { get; set; }
        [XmlElement(ElementName = "CHECKSUM")]
        public string CHECKSUM { get; set; }
    }
}
