using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class NotificationModel
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public List<ListKPI> KPI { get; set; } = null!;
        public short? NotificationOption { get; set; } 
        public string Emails { get; set; } = null!;
        public string? NotificationAlias { get; set; }
        public bool? IsActive { get; set; }
        public bool IsCheck { get; set; }

    }
    public class NotificationListRequest
    {
        public string? Name { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
    public class ListKPI
    {
        public string KPIName { get; set; } = null!;
    }
}
