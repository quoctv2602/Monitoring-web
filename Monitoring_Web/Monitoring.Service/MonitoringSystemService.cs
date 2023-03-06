using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitoring.Data;
using Monitoring.Data.IRepository;
using Monitoring.Model;
using Monitoring.Service.IService;

namespace Monitoring.Service
{
    public class MonitoringSystemService:IMonitoringSystemService
    {
        private readonly IMonitoringSystemRepository _monitoringSystemRepository;
        public MonitoringSystemService(IMonitoringSystemRepository monitoringSystemRepository)
        {
            _monitoringSystemRepository = monitoringSystemRepository;
        }
        public IList<MonitoringSystem> GetMonitoringSystems()
        {
            return _monitoringSystemRepository.GetMonitoringSystemsAsync();
        }
    }
}
