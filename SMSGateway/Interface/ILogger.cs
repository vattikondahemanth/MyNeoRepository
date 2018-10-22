using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSGateway.Interface
{
    public interface ILogger
    {
        void InsertIntoSchedulerLog(int logType, string methodName, string request, string response, string schedulerName);
    }
}
