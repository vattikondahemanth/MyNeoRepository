using Autofac;
using SMSGateway.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static SMSGateway.DIFactory;

namespace SMSGateway
{
    public class SMSSender : ISMSSender
    {
        private readonly ICustomDependencyResolver resolver;
        public SMSSender(ICustomDependencyResolver _resolver)
        {
            resolver = _resolver;
        }
        public IGateWay GetSMSProvider(SMSProviders sMSProviders)
        {
            IGateWay sMSSender;
            switch (sMSProviders)
            {
                case SMSProviders.SBI:
                    sMSSender = resolver.Resolve<SBIGateway>();
                    break;
                case SMSProviders.ACL:
                    sMSSender = resolver.Resolve<ACLGateway>();
                    break;
                default:
                    sMSSender = null;
                    break;
            }
            return sMSSender;
        }

        public  async Task<bool> InsertSentItemAsync(int messageId, string mobileNo, string smsMessage, string subject, DateTime transDatetime, string status, string priority)
        {
            bool sentStatus = false;
            var con = Program.SqlManager.GetSqlConnection;
            var cmd = resolver.Resolve<SqlCommand>();
            ILogger _logger = resolver.Resolve<ILogger>();
            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "USP_InsertSentMsg";
                cmd.Parameters.Add("@MessageId", SqlDbType.Int).Value = messageId;
                cmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = mobileNo;
                cmd.Parameters.Add("@SMSMessage", SqlDbType.VarChar).Value = smsMessage;
                cmd.Parameters.Add("@Subject", SqlDbType.VarChar).Value = subject;
                cmd.Parameters.Add("@TransDateTime", SqlDbType.DateTime).Value = transDatetime;
                cmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = status;
                cmd.Parameters.Add("@Priority", SqlDbType.VarChar).Value = priority;
                cmd.Connection = con;
                await cmd.ExecuteNonQueryAsync();
                sentStatus = true;
            }
            catch (Exception ex)
            {
                sentStatus = false;
                _logger.InsertIntoSchedulerLog(1, "InsertSentItem", "", Convert.ToString(ex), "SMSGateway");
            }
            finally
            {

                
            }

            return sentStatus;
        }

       

        public IList<SMSData> GetOutboxMessageAsync()
        {
            ILogger _logger = resolver.Resolve<ILogger>();
            DataSet ds = resolver.Resolve<DataSet>();
            IList<SMSData> objListSMS = resolver.Resolve<IList<SMSData>>();
            SqlConnection con = Program.SqlManager.GetSqlConnection;
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;
                cmd.CommandText = "USP_GetOutboxMsg";
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        SMSData objSMS = new SMSData();
                        objSMS.MessageId = Convert.ToInt32(ds.Tables[0].Rows[i]["MESSAGEID"]);
                        objSMS.MobileNumber = ds.Tables[0].Rows[i]["MOBILENUMBER"].ToString();
                        objSMS.TransDatetime = ds.Tables[0].Rows[i]["TRANSDATETIME"].ToString();
                        objSMS.SMSMessage = ds.Tables[0].Rows[i]["SMSMESSAGE"].ToString();
                        objSMS.Subject = ds.Tables[0].Rows[i]["SUBJECT"].ToString();
                        objSMS.Status = ds.Tables[0].Rows[i]["STATUS"].ToString();
                        objSMS.Priority = ds.Tables[0].Rows[i]["PRIORITY"].ToString();
                        objSMS.ServiceProvider = Convert.ToInt32(ds.Tables[0].Rows[i]["SERVICEPROVIDER"] == DBNull.Value ? 0 : ds.Tables[0].Rows[i]["SERVICEPROVIDER"]);
                        objListSMS.Add(objSMS);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.InsertIntoSchedulerLog(1, "GetOutboxMessage", "", Convert.ToString(ex), "SMSGateway");
            }
            finally
            {
                
            }
            return objListSMS;
        }
    }
}
