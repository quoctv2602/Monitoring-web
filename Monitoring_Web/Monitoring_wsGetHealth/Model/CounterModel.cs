using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Monitoring_Common.Service.WindowCounters;

namespace Monitoring_wsGetHealth.Model
{
    
    public class MonitoringSystem
    {
        public string MarchineName { get; set; }
        public string IpAddress { get; set; }
        public int CPUInfo { get; set; }
        public int MemoryInfo { get; set; }
        public int StorageInfo { get; set; }
       

    }
    public class MonitoringRespone
    {
        public MonitoringSystem result { get; set; }
        public List<CounterDetail> detail { get; set; }
        public List<DiskModel> disk { get; set; }
        public TransferModel Transfer { get; set; }
        public TransactionEDItoASCIIModel EDItoASCII { get; set; }
        public string requestID { get; set; }

        public string requestTime { get; set; }
        public string responseTime { get; set; }

        public string contentData { get; set; }

        public int Status { get; set; }
        public string Message { get; set; }
    }
    public class CounterDetail {

        public string ProcessName { get; set; }
       // public int CountInstance { get; set; }
        //public int CPUInfo { get; set; }
        //public int MemoryInfo { get; set; }
        //public int StorageInfo { get; set; }
        public string Status { get; set; }
    }

    public class DiskModel
    {
        public string DriveName { get; set; }
        public string VolumeLabel { get; set; }
        public long TotalSize { get; set; }
        public long TotalFreeSpace { get; set; }
    }
}
