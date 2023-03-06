using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class DashboardSystemHealthModel
    {
        public int NodeID { get; set; }
        public int EnvironmentID { get; set; }
        public string EnvironmentName { get; set; } = null!;
        public string MachineName { get; set; } = null!;
        public int? Data { get; set; } = null!;
        public string Label { get; set; } = null!;
        public Guid? RequestID { get; set; }
        public int? Threshold { get; set; }
        public string? DateString { get; set; } = null!;
    }
}
