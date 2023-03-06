using System;
using System.Collections.Generic;
using static Monitoring_Common.Service.WindowCounters;

namespace HealthMeasurement.Api.Models
{
    public class MonitoringSystem
    {
        public string MarchineName { get; set; }
        public string IpAddress { get; set; }
        public int CPUInfo { get; set; }
        public int MemoryInfo { get; set; }
        public int StorageInfo { get; set; }
       
      
      
    }
    public class MonitoringDetail  {
        public string ProcessName { get; set; }
      //  public int CountInstance { get; set; }
        public string Status { get; set; }



    }

    public class MonitoringRespone
    {
        public MonitoringSystem result { get; set; }
        public List<MonitoringDetail> detail { get; set; }
        public List<DiskModel> disk { get; set; }
        public TransferModel Transfer { get; set; }
        public TransactionEDItoASCIIModel EDItoASCII { get; set; }
        public string RequestID { get; set; }
    }
   
}
