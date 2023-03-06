using Monitoring.Model.Entity;
using Monitoring.Model.Model;

namespace Monitoring.Data.IRepository
{
    public interface IIntegrationAPIRepository
    {
        Task<PagedResult<IntegrationAPIModel>> GetListIntegrationAPI(IntegrationAPIRequest request);
        Task<bool> CreateIntegrationAPI(CreateIntegrationAPIRequest request);
        Task<bool> UpdateIntegrationAPI(UpdateIntegrationAPIRequest request);
        Task<IntegrationAPIModelEdit> GetIntegrationAPIEdit(int id);
        Task<int> GetIntegrationAPI(string machineName, int environmentID);
        Task<List<Sys_Environment>> GetEnvironment();
        Task<string> DeleteIntegrationAPI(List<IntegrationAPIModel> request);
        Task<List<Sys_NodeType>> GetNodeType();
        Task<bool> UpdateAPITransaction(UpdateAPITransactionRequest request);
        Task<bool> UpdateIsActiveNodeManagament(int environmentID, int? nodeType, bool isActive);
        Task<bool> UpdateIsDefaultNode(int environmentID);
    }
}
