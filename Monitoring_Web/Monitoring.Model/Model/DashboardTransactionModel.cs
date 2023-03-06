using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class DashboardTransactionModel
    {
        public List<DashboardTransaction_TableModel> TableData { get; set; } = null!;
        public List<DashboardTransaction_ColumnChartModel> ColumnChartData { get; set; } = null!;
        public List<DashboardTrasaction_PendingGraphModel> PendingGraphData { get; set; } = null!;
        public int? ThresholdPendingGraph { get; set; }
    }
}
