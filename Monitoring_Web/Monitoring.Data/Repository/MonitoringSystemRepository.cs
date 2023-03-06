using Microsoft.EntityFrameworkCore;
using Monitoring.Model;
using Monitoring.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitoring.Data.IRepository;

namespace Monitoring.Data.Repository
{
    public class MonitoringSystemRepository : IMonitoringSystemRepository
    {
        private readonly MonitoringContext _monitoringContext;
        public MonitoringSystemRepository(MonitoringContext monitoringContext)
        {
            _monitoringContext = monitoringContext;
        }

        public IList<MonitoringSystem> GetMonitoringSystemsAsync()
        {
            try
            {
                return _monitoringContext.MonitoringSystems.ToListAsync().Result;
            }
            catch (Exception ex)
            {
                return new List<MonitoringSystem>();
            }
        }
    }
}
