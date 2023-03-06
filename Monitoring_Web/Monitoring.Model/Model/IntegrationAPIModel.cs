namespace Monitoring.Model.Model
{
    public class IntegrationAPIModel
    {
        public int ID { get; set; }
        public int? EnvironmentId { get; set; }
        public string? EnvironmentName { get; set; }
        public string? MachineName { get; set; }
        public string? HealthMeasurementKey { get; set; }
        public string? Appid { get; set; }
        public string? domain_SystemHealth { get; set; }
        public Int16? NodeType { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsCheck { get; set; }
        public bool? IsDefaultNode { get; set; }
    }
    public class IntegrationAPIModelEdit
    {
        public int ID { get; set; }
        public int? EnvironmentID { get; set; }
        public string? MachineName { get; set; }
        public string? HealthMeasurementKey { get; set; }
        public string? Appid { get; set; }
        public string? domain_SystemHealth { get; set; }
        public Int16? NodeType { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? ServiceList { get; set; }
        public bool? IsDefaultNode { get; set; }
    }
}
