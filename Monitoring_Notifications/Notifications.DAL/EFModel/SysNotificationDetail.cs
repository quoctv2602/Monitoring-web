using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL.EFModel
{
    public partial  class SysNotificationDetail
    {
        public int Id { get; set; }
        public string Emails { get; set; }
        public string NotificationAlias { get; set; }
         
    }
}
