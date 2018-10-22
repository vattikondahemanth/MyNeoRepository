using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Data;
using System.Net;
using Newtonsoft.Json;
using SMSGateway.Interface;
using static SMSGateway.DIFactory;
using Autofac;

namespace SMSGateway
{
    class Program
    {
        public static string connectionString;
        public static string SBI_userid;
        public static string SBI_password;
        public static string SBI_sendernumber;
        public static string SBI_sendername;
        public static string SBI_category;
        public static string SBI_smstype;
        public static string SBI_clientmsgid;
        public static string ACL_enterpriseid;
        public static string ACL_subEnterpriseid;
        public static string ACL_pusheid;
        public static string ACL_pushepwd;
        public static string ACL_msisdn;
        public static string ACL_sender;
        public static string ACL_msgtext;
        public static string SBI_url;
        public static IContainer resolver;
        public static SqlManager SqlManager;
        public static bool IsSqlConnectionClosed;

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DIFactory());
            resolver = builder.Build();

            LoadConfigurationSettings();
            SendMessage().Wait();
            if (!IsSqlConnectionClosed) IsSqlConnectionClosed = SqlManager.CloseSqlConnection;
        }

        private static async Task<bool> SendMessage()
        {
            ISMSSender _smsSender = resolver.Resolve<ISMSSender>();
            ILogger _logger = resolver.Resolve<ILogger>();
            
            bool status = false;
            IList<Task<SMSResponse>> TaskList = resolver.Resolve<IList<Task<SMSResponse>>>();
            IList<Task<bool>> SentMsgTaskList = resolver.Resolve<IList<Task<bool>>>();
            try
            {
                IList<SMSData> liSMSData = _smsSender.GetOutboxMessageAsync().ToList();
                foreach (SMSData objSMS in liSMSData)
                {

                    var _smsprovider = _smsSender.GetSMSProvider((SMSProviders)objSMS.ServiceProvider);
                    TaskList.Add(_smsprovider.SendSMS(objSMS));
                }
                SMSResponse[] ResponseList = await Task.WhenAll(TaskList);
                foreach (SMSResponse Response in ResponseList)
                {
                    SMSData SMS = liSMSData.Where(t => t.MessageId == Response.MessageID).FirstOrDefault();
                    SentMsgTaskList.Add(_smsSender.InsertSentItemAsync(SMS.MessageId, SMS.MobileNumber, SMS.SMSMessage, SMS.Subject, Convert.ToDateTime(SMS.TransDatetime), Convert.ToInt32(Response.IsSMSSent).ToString(), SMS.Priority));
                }

                await Task.WhenAll(SentMsgTaskList);
            }
            catch (Exception ex)
            {
                _logger.InsertIntoSchedulerLog(1, "SendMessage", "", Convert.ToString(ex.ToString()), "SMSGateway");
            }

            finally
            {
                IsSqlConnectionClosed = SqlManager.CloseSqlConnection;
            }
            return status;
        }

        private static void LoadConfigurationSettings()
        {
            SqlManager = resolver.Resolve<SqlManager>();
            connectionString = ConfigurationManager.ConnectionStrings["SBI_MF"].ToString();
            var appSettings = ConfigurationManager.AppSettings;
            SBI_userid = appSettings.AllKeys.Contains("SBIuserid") ? appSettings.Get("SBIuserid") : "";
            SBI_password = appSettings.AllKeys.Contains("SBIpassword") ? appSettings.Get("SBIpassword") : "";
            SBI_sendernumber = appSettings.AllKeys.Contains("SBIsendernumber") ? appSettings.Get("SBIsendernumber") : "";
            SBI_sendername = appSettings.AllKeys.Contains("SBIsendername") ? appSettings.Get("SBIsendername") : "";
            SBI_category = appSettings.AllKeys.Contains("SBIcategory") ? appSettings.Get("SBIcategory") : "";
            SBI_smstype = appSettings.AllKeys.Contains("SBIsmstype") ? appSettings.Get("SBIsmstype") : "";
            SBI_clientmsgid = appSettings.AllKeys.Contains("SBIclientmsgid") ? appSettings.Get("SBIclientmsgid") : "";
            SBI_url = appSettings.AllKeys.Contains("SBIURL") ? appSettings.Get("SBIURL") : "";

            ACL_enterpriseid = appSettings.AllKeys.Contains("ACLenterpriseid") ? appSettings.Get("ACLenterpriseid") : "";
            ACL_subEnterpriseid = appSettings.AllKeys.Contains("ACLsubEnterpriseid") ? appSettings.Get("ACLsubEnterpriseid") : "";
            ACL_pusheid = appSettings.AllKeys.Contains("ACLpusheid") ? appSettings.Get("ACLpusheid") : "";
            ACL_pushepwd = appSettings.AllKeys.Contains("ACLpushepwd") ? appSettings.Get("ACLpushepwd") : "";
            ACL_msisdn = appSettings.AllKeys.Contains("ACLmsisdn") ? appSettings.Get("ACLmsisdn") : "";
            ACL_sender = appSettings.AllKeys.Contains("ACLsender") ? appSettings.Get("ACLsender") : "";
            ACL_msgtext = appSettings.AllKeys.Contains("ACLmsgtext") ? appSettings.Get("ACLmsgtext") : "";
            

        }

        
    }
}
