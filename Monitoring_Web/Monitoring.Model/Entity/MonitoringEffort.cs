using System;
using System.Collections.Generic;

namespace Monitoring.Model
{
    public partial class MonitoringEffort
    {
        public string? MachineName { get; set; }
        public string? Ipaddress { get; set; }
        public string? AppName { get; set; }
        public DateTime? BeginTransaction { get; set; }
        public DateTime? EndTransaction { get; set; }
        public DateTime? BeginFileTransfer { get; set; }
        public DateTime? EndFileTransfer { get; set; }
    }
}
