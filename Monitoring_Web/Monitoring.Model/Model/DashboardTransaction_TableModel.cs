using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class DashboardTransaction_TableModel
    {
        public int? RowNum { get; set; }
        public Guid TransactionKey { get; set; }
        public string? ErrorStatus { get; set; }
        public string? MonitoredStatus { get; set; }
    }
}
