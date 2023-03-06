using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class NodeSettingModel
    {
        public int ID { get; set; }
        public string? NodeName { get; set; }
        public int EnvironmentID { get; set; }
        public string? EnvironmentName { get; set; } = null!;
        public string? Description { get; set; }
        public string? ServiceList { get; set; }
        public string? NotificationEmail { get; set; }
        public string? ReportEmail { get; set; }
        public int? NodeType { get; set; }
        public bool? IsDefault { get; set; }
    }
}
