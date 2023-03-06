
using HealthMeasurement.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Monitoring_Common.Service.WindowCounters;

namespace HealthMeasurement.Api.Service
{
    public interface IHealthMeasurementService
    {
        MonitoringSystem GetDataMonitor();
        List<MonitoringDetail> GetDetailDataMonitor(List<ProcessModel> list);
        TransferModel GetTransfer();

        TransactionEDItoASCIIModel GetAppAndWarningEDItoASCII();


    }
}
