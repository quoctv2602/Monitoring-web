using System;
using System.Collections.Generic;

namespace Monitoring.Model
{
    public partial class MonitoringSystem
    {
        public Guid Id { get; set; }
        public string? MachineName { get; set; }
        public string? Ipaddress { get; set; }
        public int? Cpuinfo { get; set; }
        public int? Cpuover { get; set; }
        public int? MemoryInfo { get; set; }
        public int? MemoryOver { get; set; }
        public DateTime? RequestTime { get; set; }
        public DateTime? ResponseTime { get; set; }
        public int? StorageInfo { get; set; }
        public int? StorageOver { get; set; }
        public int? NumberOfTransactionFail { get; set; }
        public int? TransactionFailOver { get; set; }
        public string? ContentData { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
