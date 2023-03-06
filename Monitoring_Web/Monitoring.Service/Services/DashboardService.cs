using Monitoring.Data.IRepository;
using Monitoring.Data.Repository;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Service.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;
        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<NodeSettingModel>> ListNodeSettings()
        {
            return await _repository.ListNodeSettings();
        }

        public async Task<List<DashboardSystemHealthModel>> DataDashboardSystemHealth(List<NodeSettingModel> nodeSettings, int monitoringType, int topSelect)
        {
            return await _repository.DataDashboardSystemHealth(nodeSettings, monitoringType, topSelect);
        }

        public async Task<List<ServiceModel>> ServiceList(List<ServiceListRequestModel> nodeSettings)
        {
            return await _repository.ServiceList(nodeSettings);
        }

        public async Task<List<DashboardSystemHealth_KPIFreeDiskModel>> DataDashboardSystemHealth_KPIFreeDisk(List<NodeSettingModel> nodeSettings, int monitoringType, int topSelect)
        {
            return await _repository.DataDashboardSystemHealth_FreeDisk(nodeSettings, monitoringType, topSelect);
        }

        public async Task<List<DashboardTransaction_TableModel>> DashboardTransaction_Table(int environmentID, string cipFlow, string lastest, int topSelect)
        {
            return await _repository.DashboardTransaction_Table(environmentID, cipFlow, lastest, topSelect);
        }
        public async Task<List<DashboardTransaction_ColumnChartModel>> DashboardTransaction_ColumnChart(int environmentID, string cipFlow, string lastest)
        {
            return await _repository.DashboardTransaction_ColumnChart(environmentID, cipFlow, lastest);
        }
        public async Task<List<DashboardTrasaction_PendingGraphModel>> DashboardTransaction_PendingGraph(int environmentID, string cipFlow, string lastest, int topSelect)
        {
            return await _repository.DashboardTransaction_PendingGraph(environmentID, cipFlow, lastest, topSelect);
        }
        public async Task<int?> DashboardTransaction_ThresholdPendingGraph(int environmentID)
        {
            return await _repository.DashboardTransaction_ThresholdPendingGraph(environmentID);
        }
        public async Task<List<EnvironmentModel>> Environments_ByNodeType(int? nodeType)
        {
            return await _repository.Environments_ByNodeType(nodeType);
        }
    }
}
