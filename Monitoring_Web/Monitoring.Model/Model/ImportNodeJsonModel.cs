using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class ImportNodeJsonModel
    {
    }
    public partial class NodeSettingsJson
    {
        public int ID { get; set; }
        public Int16? NodeType { get; set; }
        public string? NodeName { get; set; }
        public int EnvironmentID { get; set; }
        public string? MachineName { get; set; }
        public string? Description { get; set; }
        public string? ServiceList { get; set; }
        public string? NotificationEmail { get; set; }
        public string? ReportEmail { get; set; }
        public string? NotificationAlias { get; set; }
        public string? ReportAlias { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsActive { get; set; }
        public string? HealthMeasurementKey { get; set; }
        public string? Appid { get; set; }
        public string? domain_SystemHealth { get; set; }
        public List<ThresholdRuleExport>? ListThresholdRule { get; set; }

    }
}
