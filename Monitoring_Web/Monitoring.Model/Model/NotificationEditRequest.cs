using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class NotificationEditRequest
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public short? NotificationOption { get; set; }
        public List<MonitoringEdit> KPI { get; set; } = null!;
        public string Emails { get; set; } = null!;
        public string NotificationAlias { get; set; } = null!;
        public int? UpdateBy { get; set; }
    }
    public class MonitoringEdit
    {
        public int? ID { get; set; }
        public int? MonitoringId { get; set; }
    }
    public class ToggleNotificationRequest
    {
        public int ID { get; set; }
        public bool IsActive { get; set; }
    }
}
