using Monitoring.Data.IRepository;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring_Common.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Service.Services
{
    public class IntegrationAPIService: IIntegrationAPIService
    {
        private readonly IIntegrationAPIRepository _integrationAPIRepository;
        public IntegrationAPIService(IIntegrationAPIRepository integrationAPIRepository)
        {
            _integrationAPIRepository = integrationAPIRepository;
        }
        public async Task<PagedResult<IntegrationAPIModel>> GetListIntegrationAPI(IntegrationAPIRequest request)
        {
            return await _integrationAPIRepository.GetListIntegrationAPI(request);
        }
        public async Task<ApiResult<string>> CreateIntegrationAPI(CreateIntegrationAPIRequest request)
        {
            var integrationId = await _integrationAPIRepository.GetIntegrationAPI(request.MachineName, request.EnvironmentID);
            if (integrationId != 0 )
            {
                return new ApiErrorResult<string>("The same node has defined.");
            }
            if (request.IsDefaultNode == true)
            {
                var defaultNode = await _integrationAPIRepository.UpdateIsDefaultNode(request.EnvironmentID);
                if (defaultNode == false)
                {
                    return new ApiErrorResult<string>("Update IsDefaultNode error");
                }
            }
            var result=  await _integrationAPIRepository.CreateIntegrationAPI(request);
            if (result == false)
            {
                return new ApiErrorResult<string>("Node create error");
            }
            return new ApiSuccessResult<string>();
        }
        public async Task<ApiResult<string>> UpdateIntegrationAPI(UpdateIntegrationAPIRequest request)
        {
            if (request.IsDefaultNode == true)
            {
                var defaultNode = await _integrationAPIRepository.UpdateIsDefaultNode(request.EnvironmentID);
                if (defaultNode == false)
                {
                    return new ApiErrorResult<string>("Update IsDefaultNode error");
                }
            }
            var dt= await _integrationAPIRepository.UpdateIntegrationAPI(request);
            if (dt == false)
            {
                return new ApiErrorResult<string>("Update integration error");
            }
            return new ApiSuccessResult<string>();
        }
        public async Task<IntegrationAPIModelEdit> GetIntegrationAPIEdit(int id)
        {
            return await _integrationAPIRepository.GetIntegrationAPIEdit(id);
        }
        public async Task<List<Sys_Environment>> GetEnvironment()
        {
            return await _integrationAPIRepository.GetEnvironment();
        }
        public async Task<string> DeleteIntegrationAPI(List<IntegrationAPIModel> request)
        {
            return await _integrationAPIRepository.DeleteIntegrationAPI(request);
        }   
        public async Task<List<Sys_NodeType>> GetNodeType()
        {
            return await _integrationAPIRepository.GetNodeType();
        }
        public async Task<ApiResult<string>> UpdateAPITransaction(UpdateAPITransactionRequest request)
        {
            var dt= await _integrationAPIRepository.UpdateAPITransaction(request);
            if(dt==false)
            {
                return new ApiErrorResult<string>("Update integration error");
            }
            return new ApiSuccessResult<string>();
        }
        public async Task<bool> UpdateIsActiveNodeManagament(int environmentID, int? nodeType, bool isActive)
        {
            return await _integrationAPIRepository.UpdateIsActiveNodeManagament(environmentID, nodeType, isActive);
        }
    }
}

