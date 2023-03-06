using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class ServiceModel
    {
        public string ServiceName { get; set; } = null!;
        public string? NodeName { get; set; } = null!;
        public int EnvironmentID { get; set; }
        public string? Status { get; set; } = null!;
        public Guid? RequestID { get; set; } = null!;
        public int? Instance { get; set; }
    }
}
