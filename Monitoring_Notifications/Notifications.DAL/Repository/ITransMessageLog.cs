using Notifications.DAL.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL.Repository
{
    public interface ITransMessageLog
    {
        IList<TransMessageLog> GetTransMessageLogs(int status);
        IList<TransMessageLog> GetTransMessageLogs(string machineName, int environmentId, int sendType);
        //TransMessageLog GetTransMessageLog(string Id);
        void Add(TransMessageLog transMessageLog);
        void Update(TransMessageLog transMessageLog);
    }
}
