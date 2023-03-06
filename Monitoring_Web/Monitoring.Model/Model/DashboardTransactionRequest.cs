using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class DashboardTransactionRequest
    {
        public int EnvironmentID { get; set; }
        public string CIPFlow { get; set; } = null!;
        public string Lastest { get; set; } = null!;
        public string ConnectionId { get; set; } = null!;
    }
}
