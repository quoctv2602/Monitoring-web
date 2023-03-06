using Monitoring.Model.Model;

namespace Monitoring.Model.Model
{
    public class NodeSettingRequest
    {
        public Int16? NodeType { get; set; }
        public string? NodeName { get; set; }
        public int EnvironmentID { get; set; }
        public string? Description { get; set; }
        public string? ServiceList { get; set; }
        public string? NotificationEmail { get; set; }
        public string? ReportEmail { get; set; }
        public string? NotificationAlias { get; set; }
        public string? ReportAlias { get; set; }
        public string? domain_SystemHealth { get; set; }
        public string? Appid { get; set; }
        public string? HealthMeasurementKey { get; set; }
        
        public List<ThresholdRuleRequest>? ListThresholdRuleRequest { get; set; }
    }
    public class NodeSettingIdRequest
    {
        public int Id { get; set; }
    }
    public class NodeSettingEditRequest
    {
        public int ID { get; set; }
        public Int16 NodeType { get; set; }
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
        public string? HealthMeasurementKey { get; set; }
        public string? Appid { get; set; }
        public string? domain_SystemHealth { get; set; }
        public List<ThresholdRuleEditRequest>? ListThresholdRuleEdit { get; set; }
    }

    public class SettingsRequest
    {
        public string? NodeName { get; set; }
        public string? IsActive { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
    public class PagingRequestBase
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }

    public partial class ThresholdRuleEditRequest
    {
        public int ID { get; set; }
        public int Node_Setting { get; set; }
        public int EnvironmentID { get; set; }
        public string? MachineName { get; set; } 
        public int MonitoringType { get; set; }
        public string? MonitoringName { get; set; }
        public byte? Condition { get; set; }
        public int? Threshold { get; set; }
        public int? ThresholdCounter { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
