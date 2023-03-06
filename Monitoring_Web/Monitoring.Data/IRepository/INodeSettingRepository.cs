using Monitoring.Model.Entity;
using Monitoring.Model.Model;

namespace Monitoring.Data.IRepository
{
    public interface  INodeSettingRepository
    {
        Task<List<Sys_Monitoring>> GetSysMonitoring();
        Task<List<EnvironmentModel>> GetSysEnvironment();
        Task<int> CreateNodeSetting(NodeSettingRequest request);
        Task<bool> CreateThresholdRule(List<ThresholdRuleRequest> request, int id, int environmentID, string nodeName);
        Task<int> GetNodeName(string nodeName, int environmentID);
        Task<bool> UpdateNodeSetting(NodeSettingEditRequest request);
        Task<bool> UpdateThresholdRule(List<ThresholdRuleEditRequest> request, int id, int environmentID, string nodeName, int isUpdateNode);
        Task<PagedResult<NodeSettings>> GetListSysNodeSetting(SettingsRequest request);
        Task<List<Sys_Integration_API>> GetListIntegrationAPI(int id);
        Task<NodeSettingsEdit> GetSysNodeSetting(int id);
        Task<List<Sys_Threshold_Rule>> GetSysThresholdRule(int Node_Setting);
        Task<List<KPIListExportModel>> GetDataExportJson(string id);
        Task<bool> UpdateIsActiveNode(NodeSettings request);
        Task<int> CheckNodeName(string nodeName, int environmentID, int id);
        Task<int> checkExitNode(string nodeName, int environmentID);
        Task<int> CreateNodeSettingImport(KPIListExportModel request);
        Task<bool> CreateThresholdRuleImport(List<ThresholdRuleExport> request, int id, int environmentID, string nodeName);
        Task<int?> GetNodeType(int id);
        Task<bool> DeleteIntegrationAPI(int environmentID, int? nodeType);
    }
}
