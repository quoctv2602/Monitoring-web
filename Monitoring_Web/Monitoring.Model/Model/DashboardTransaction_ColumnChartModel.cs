using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class DashboardTransaction_ColumnChartModel
    {
        public Guid RequestID { get; set; }
        public string ErrorStatus { get; set; } = null!;
        public int? NumberOfTransactions { get; set; }
    }
}
