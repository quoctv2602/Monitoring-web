using Monitoring.Model.Entity;
using Monitoring.Model.Model;
using Monitoring_Common.Common;

namespace Monitoring.Service.IService
{
    public interface INodeSettingService
    {
        Task<List<Sys_Monitoring>> GetSysMonitoring();
        Task<NodeSetingsModel> GetSysEnvironment();
        Task<ApiResult<string>> CreateNodeSetting(NodeSettingRequest request);
        Task<ApiResult<string>> UpdateNodeSetting(NodeSettingEditRequest request);
        Task<PagedResult<NodeSettings>> GetListSysNodeSetting(SettingsRequest request);
        Task<NodeSettingsEdit> GetSysNodeSetting(int id);
        Task<List<Sys_Threshold_Rule>> GetSysThresholdRule(int node_settings);
        Task<List<KPIListExportModel>> GetDataExportJson(string id);
        Task<bool> UpdateIsActiveNode(NodeSettings request);
        Task<ApiResult<string>> CreateNodeSettingImport(KPIListExportModel request);
        Task<int?> GetNodeType(int id);
    }
}
