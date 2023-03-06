namespace Monitoring.Model.Model
{
    public class IntegrationAPIRequest
    {
        public string? MachineName { get; set; }
        public string? EnvironmentName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
    public class CreateIntegrationAPIRequest
    {
        public int EnvironmentID { get; set; }
        public string? MachineName { get; set; }
        public string? HealthMeasurementKey { get; set; }
        public string? Appid { get; set; }
        public string? domain_SystemHealth { get; set; }
        public Int16? NodeType { get; set; }
        public bool IsActive { get; set; }
        public string? ServiceList { get; set; }
        public bool? IsDefaultNode { get; set; }
    }
    public class UpdateIntegrationAPIRequest
    {
        public int ID { get; set; }
        public int EnvironmentID { get; set; }
        public string? MachineName { get; set; }
        public string? HealthMeasurementKey { get; set; }
        public string? Appid { get; set; }
        public string? domain_SystemHealth { get; set; }
        public Int16 NodeType { get; set; }
        public string? NodeTypeName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? ServiceList { get; set; }
        public bool? IsDefaultNode { get; set; }
    }
    public class UpdateAPITransactionRequest
    {
        public int EnvironmentId { get; set; }
        public string? HealthMeasurementKey { get; set; }
        public string? Appid { get; set; }
        public string? domain_SystemHealth { get; set; }
        public Int16? NodeType { get; set; }
    }
}
