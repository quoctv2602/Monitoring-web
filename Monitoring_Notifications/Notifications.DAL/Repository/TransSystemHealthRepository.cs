using Notifications.DAL.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL.Repository
{
    public class TransSystemHealthRepository : ITransSystemHealth
    {
        public IList<TransSystemHealth> GetTransSystemHealths()
        {
            return new List<TransSystemHealth>();
        }
    }
}
