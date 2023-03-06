using Notifications.DAL.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL.Repository
{
    public class TransMessageLogRepository : ITransMessageLog
    {
        public IList<TransMessageLog> GetTransMessageLogs(int status)
        {
            return new List<TransMessageLog>();
        }
        public IList<TransMessageLog> GetTransMessageLogs(string machineName, 
                            int environmentId, int sendType)
        {
            return new List<TransMessageLog>();
        }
        public void Add(TransMessageLog transMessageLog)
        {
        }
        public void Update(TransMessageLog transMessageLog)
        {
        }
    }
}
