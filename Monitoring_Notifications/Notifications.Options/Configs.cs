using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Options
{
    public class Configs
    {
        public AppSettings AppSettings { get; set; }
        public DatabaseSettings DatabaseSettings { get; set; }
    }
    public class AppSettings
    {
        public int NumThreadsForJob { get; set; }
        public string NotificationJobCron { get; set; }
        public string SummaryReportJobCron { get; set; }
        public string ArchiveDataJobCron { get; set; }
        public string DayCount { get; set; }
        public int MaxRetryDBCount { get; set; }
        public int MaxDelayRetryingDB { get; set; }
        public int CommandTimeout { get; set; }
        public int EmailRetryCount {get; set; }
        public int EmailDelayRetryMiliSeconds { get; set; }
        public int HoursOfYesterday { get; set; }
        public string NotificationMonitoringURL { get; set; }
    }
    public class DatabaseSettings
    {
        public string NotificationContext { get; set; }
    }
}
