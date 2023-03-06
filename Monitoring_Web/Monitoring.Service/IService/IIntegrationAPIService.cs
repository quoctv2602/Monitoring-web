using Monitoring.Model.Entity;
using Monitoring.Model.Model;
using Monitoring_Common.Common;

namespace Monitoring.Service.IService
{
    public interface IIntegrationAPIService
    {
        Task<PagedResult<IntegrationAPIModel>> GetListIntegrationAPI(IntegrationAPIRequest request);
        Task<ApiResult<string>> CreateIntegrationAPI(CreateIntegrationAPIRequest request);
        Task<ApiResult<string>> UpdateIntegrationAPI(UpdateIntegrationAPIRequest request);
        Task<IntegrationAPIModelEdit> GetIntegrationAPIEdit(int id);
        Task<List<Sys_Environment>> GetEnvironment();
        Task<string> DeleteIntegrationAPI(List<IntegrationAPIModel> request);
        Task<List<Sys_NodeType>> GetNodeType();
        Task<ApiResult<string>> UpdateAPITransaction(UpdateAPITransactionRequest request);
        Task<bool> UpdateIsActiveNodeManagament(int environmentID, int? nodeType, bool isActive);
    }
}
