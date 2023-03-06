using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class DashboardSystemHealthRequest
    {
        public List<NodeSettingModel> nodeSettings { get; set; } = null!;
        public int monitoringType { get; set; }
        public string connectionId { get; set; } = null!;
    }
}
