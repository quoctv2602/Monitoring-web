using Monitoring.Data.IRepository;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring_Common.Common;
using Monitoring_Common.Security;

namespace Monitoring.Service.Services
{
    public class NodeSettingService : INodeSettingService
    {
        private readonly INodeSettingRepository _nodeSettingRepository;
        public NodeSettingService(INodeSettingRepository nodeSettingRepository)
        {
            this._nodeSettingRepository = nodeSettingRepository;
        }
        public async Task<List<Sys_Monitoring>> GetSysMonitoring()
        {
            return await _nodeSettingRepository.GetSysMonitoring();
        }
        public async Task<NodeSetingsModel> GetSysEnvironment()
        {
            NodeSetingsModel nodeSettingsModel = new NodeSetingsModel();
            var listEnvir = await _nodeSettingRepository.GetSysEnvironment();
            foreach (var itemEnvir in listEnvir)
            {
                foreach (var item in itemEnvir.ListIntegrationAPI)
                {
                    if (!string.IsNullOrEmpty(item.ServiceList))
                    {
                        string[] service = item.ServiceList.Split(';');
                        List<ListService> listService = new List<ListService>();
                        for (int i = 0; i < service.Length; i++)
                        {
                            listService.Add(new ListService() { ID = item.ID, name = service[i] });
                            item.Service = listService;
                        }
                    }
                }
            }
            nodeSettingsModel.ListEnvironments = listEnvir;
            return nodeSettingsModel;
        }
        public async Task<ApiResult<string>> CreateNodeSetting(NodeSettingRequest request)
        {
            
            var isNode = await _nodeSettingRepository.checkExitNode(request.NodeName, request.EnvironmentID);
            if (isNode == 0)
            {
                return new ApiErrorResult<string>("Node is not available yet");
            }
            var isNodeName = await _nodeSettingRepository.GetNodeName(request.NodeName,request.EnvironmentID);
            if (isNodeName != 0)
            {
                return new ApiErrorResult<string>("The same node has defined.");
            }
            var id = await _nodeSettingRepository.CreateNodeSetting(request);
            if (id == 0)
            {
                return new ApiErrorResult<string>("Create node settings error");
            }
             await _nodeSettingRepository.CreateThresholdRule(request.ListThresholdRuleRequest, id, request.EnvironmentID, request.NodeName);
            return new ApiSuccessResult<string>("Add node settings success");
        }
        public async Task<ApiResult<string>> UpdateNodeSetting(NodeSettingEditRequest request)
        {
            var checkNodeName = await _nodeSettingRepository.CheckNodeName(request.NodeName, request.EnvironmentID,request.ID);
            if(checkNodeName != 0)
            {
                var nodeTP = await _nodeSettingRepository.GetNodeType(request.ID);
                if (nodeTP == 1 && request.NodeType == 2)
                {
                    var resultDelete = await _nodeSettingRepository.DeleteIntegrationAPI(request.EnvironmentID, nodeTP);
                    if (resultDelete == false)
                    {
                        return new ApiErrorResult<string>("Remove node error");
                    }
                }
                var result = await _nodeSettingRepository.UpdateNodeSetting(request);
                if (result == false)
                {
                    return new ApiErrorResult<string>("Update node settings error");
                }
                await _nodeSettingRepository.UpdateThresholdRule(request.ListThresholdRuleEdit, request.ID, request.EnvironmentID, request.NodeName,2);
                return new ApiSuccessResult<string>("Update node settings success");
            }
            var isNodeName = await _nodeSettingRepository.GetNodeName(request.NodeName, request.EnvironmentID);
            if (isNodeName != 0)
            {
                return new ApiErrorResult<string>("Node name already exist");
            }
            var nodeType = await _nodeSettingRepository.GetNodeType(request.ID);
            if (nodeType == 1 && request.NodeType == 2)
            {
                var resultDelete = await _nodeSettingRepository.DeleteIntegrationAPI(request.EnvironmentID, nodeType);
                if (resultDelete == false)
                {
                    return new ApiErrorResult<string>("Remove node error");
                }
            }
            var id = await _nodeSettingRepository.UpdateNodeSetting(request);
            if (id == false)
            {
                return new ApiErrorResult<string>("Update node settings error"); 
            }
            await _nodeSettingRepository.UpdateThresholdRule(request.ListThresholdRuleEdit, request.ID,request.EnvironmentID,request.NodeName,1);
            return  new ApiSuccessResult<string>("Update node settings success");
        }
        public async Task<PagedResult<NodeSettings>> GetListSysNodeSetting(SettingsRequest request)
        {
            return await _nodeSettingRepository.GetListSysNodeSetting(request);
        }
        public async Task<NodeSettingsEdit> GetSysNodeSetting(int id)
        {
            return await _nodeSettingRepository.GetSysNodeSetting(id);
        }
        public async Task<List<Sys_Threshold_Rule>> GetSysThresholdRule(int node_settings)
        {
            return await _nodeSettingRepository.GetSysThresholdRule(node_settings);
        }
        public async Task<List<KPIListExportModel>> GetDataExportJson(string id)
        {
            return await _nodeSettingRepository.GetDataExportJson(id);
        }
        public async Task<bool> UpdateIsActiveNode(NodeSettings request)
        {
            return await _nodeSettingRepository.UpdateIsActiveNode(request);
        }
        public async Task<ApiResult<string>> CreateNodeSettingImport(KPIListExportModel request)
        {

            var isNode = await _nodeSettingRepository.checkExitNode(request.NodeName, request.EnvironmentID);
            if (isNode == 0)
            {
                return new ApiErrorResult<string>("Node is not available yet");
            }
            var isNodeName = await _nodeSettingRepository.GetNodeName(request.NodeName, request.EnvironmentID);
            if (isNodeName != 0)
            {
                return new ApiErrorResult<string>("The same node has defined.");
            }
            var id = await _nodeSettingRepository.CreateNodeSettingImport(request);
            if (id == 0)
            {
                return new ApiErrorResult<string>("Create node settings error");
            }
            await _nodeSettingRepository.CreateThresholdRuleImport(request.ListThresholdRule, id, request.EnvironmentID, request.NodeName);
            return new ApiSuccessResult<string>("Add node settings success");
        }
        public async Task<int?> GetNodeType(int id)
        {
            return await _nodeSettingRepository.GetNodeType(id);
        }
    }
}
