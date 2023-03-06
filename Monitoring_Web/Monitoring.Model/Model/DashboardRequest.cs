using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class DashboardRequest
    {
        public DashboardSystemHealthRequest? SystemHealthRequest { get; set; }
        public DashboardTransactionRequest? TransactionRequest { get; set; }
        public string ConnectionId { get; set; } = null!;
        public int DashboardType { get; set; }
    }
}
