using Monitoring.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Service.IService
{
    public interface IDashboardService
    {
        Task<List<NodeSettingModel>> ListNodeSettings();
        Task<List<DashboardSystemHealthModel>> DataDashboardSystemHealth(List<NodeSettingModel> nodeSettings, int monitoringType, int topSelect);
        Task<List<ServiceModel>> ServiceList(List<ServiceListRequestModel> nodeSettings);
        Task<List<DashboardSystemHealth_KPIFreeDiskModel>> DataDashboardSystemHealth_KPIFreeDisk(List<NodeSettingModel> nodeSettings, int monitoringType, int topSelect);
        Task<List<DashboardTransaction_TableModel>> DashboardTransaction_Table(int enviromentID, string cipFlow, string lastest,int topSelect);
        Task<List<DashboardTransaction_ColumnChartModel>> DashboardTransaction_ColumnChart(int enviromentID, string cipFlow, string lastest);
        Task<List<DashboardTrasaction_PendingGraphModel>> DashboardTransaction_PendingGraph(int enviromentID, string cipFlow, string lastest,int topSelect);
        Task<int?> DashboardTransaction_ThresholdPendingGraph(int environmentID);
        Task<List<EnvironmentModel>> Environments_ByNodeType(int? nodeType);
    }
}
