using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class DashboardSystemHealth_KPIFreeDiskModel
    {
        public int? EnvironmentID { get; set; }
        public string? EnvironmentName { get; set; }
        public string? MachineName { get; set; }
        public string? DriveName { get; set; }
        public string? VolumeLabel { get; set; }
        public Int64? TotalSize { get; set; }
        public Int64? TotalFreeSpace { get; set; }
        public Int64? TotalUsedSpace { get; set; }
        public decimal? PercentFreeSpace { get; set; }
        public decimal? PercentUsedSpace { get; set; }
        public int? Threshold { get; set; }
        public Guid? RequestID { get; set; }
    }
}
