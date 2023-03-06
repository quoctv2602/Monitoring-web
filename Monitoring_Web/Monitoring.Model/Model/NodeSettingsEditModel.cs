using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class NodeSettingsEditModel
    {
        public int ID { get; set; }
        public string NodeName { get; set; } = null!;
        public int EnvironmentID { get; set; }
        public string MachineName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ServiceList { get; set; } = null!;
        public string NotificationEmail { get; set; } = null!;
        public string ReportEmail { get; set; } = null!;
        public DateTime CreateDate { get; set; }
       // public List<ThresholdRuleEditModel> ListThresholdRuleEdit { get; set; }
    }
    public partial class ThresholdRuleEditModel
    {
        public int ID { get; set; }
        public int Node_Setting { get; set; }
        public int EnvironmentID { get; set; }
        public string MachineName { get; set; } = null!;
        public int MonitoringType { get; set; }
        public string? MonitoringName { get; set; }
        public byte? Condition { get; set; }
        public int? Threshold { get; set; }
        public int? ThresholdCounter { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
