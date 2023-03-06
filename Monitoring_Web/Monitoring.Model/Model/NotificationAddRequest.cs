using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class NotificationAddRequest
    {
        public string Name { get; set; } = null!;
        public short? NotificationOption { get; set; }
        public List<Monitoring> KPI { get; set; } = null!;
        public string Emails { get; set; } = null!;
        public string NotificationAlias { get; set; } = null!;
        public int? CreatedBy { get; set; }
    }
    public class Monitoring
    {
        public int? MonitoringId { get; set; }
    }
}
