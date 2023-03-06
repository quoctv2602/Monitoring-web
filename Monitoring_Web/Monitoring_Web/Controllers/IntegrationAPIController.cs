using Microsoft.AspNetCore.Mvc;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring_Common.Common;
using Monitoring_Web.Filter;
using System;
using System.Net;
using static Monitoring_Common.Enum;

namespace Monitoring_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationAPIController : BaseController
    {
        private readonly IIntegrationAPIService _integrationAPIService;
        private readonly ILogger<IntegrationAPIController> _logger;
        public IntegrationAPIController(IIntegrationAPIService integrationAPIService, ILogger<IntegrationAPIController> logger)
        {
            _integrationAPIService= integrationAPIService;
            _logger = logger;
        }
        [ActionFilter(ActionId = (int)ActionEnum.kpiSettingsManageNode)]
        [HttpPost]
        [Route("createIntegrationAPI")]
        public async Task<ActionResult> CreateIntegrationAPI([FromBody] CreateIntegrationAPIRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (!string.IsNullOrWhiteSpace(request.ServiceList)) 
                {
                    request.ServiceList= request.ServiceList.Replace(',', ';');
                }
                var result = await _integrationAPIService.CreateIntegrationAPI(request);
                if (result.IsSuccessed == false)
                {
                    return Ok(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("getListIntegrationAPI")]
        public async Task<ActionResult<PagedResult<IntegrationAPIModel>>> GetListIntegrationAPI([FromBody] IntegrationAPIRequest request)
        {
            try
            {
                var list = await _integrationAPIService.GetListIntegrationAPI(request);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.kpiSettingsManageNode)]
        [HttpPost]
        [Route("getIntegrationAPIEdit")]
        public async Task<ActionResult<IntegrationAPIModelEdit>> GetIntegrationAPIEdit([FromBody] int id)
        {
            try
            {
                var list = await _integrationAPIService.GetIntegrationAPIEdit(id);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.kpiSettingsManageNode)]
        [HttpPost]
        [Route("updateIntegrationAPI")]
        public async Task<ActionResult> UpdateIntegrationAPI([FromBody] UpdateIntegrationAPIRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (!string.IsNullOrWhiteSpace(request.ServiceList))
                {
                    request.ServiceList = request.ServiceList.Replace(',', ';');
                }
                var result = await _integrationAPIService.UpdateIntegrationAPI(request);
                if (result.IsSuccessed == false)
                {
                    return Ok(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("getEnvironment")]
        public async Task<ActionResult<List<Sys_Environment>>> GetEnvironment()
        {
            try
            {
                var list = await _integrationAPIService.GetEnvironment();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.kpiSettingsManageNode)]
        [HttpPost]
        [Route("deleteIntegrationAPI")]
        public async Task<ActionResult> DeleteIntegrationAPI([FromBody] List<IntegrationAPIModel> request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _integrationAPIService.DeleteIntegrationAPI(request);
                if (result == "Error")
                {
                    return Ok(new ApiErrorResult<string>("Delete Integration API error"));
                }
                if (result == "Success")
                {
                    return Ok(new ApiSuccessResult<string>());
                }
                return Ok(new ApiErrorResult<string>(result+ " has been configured.System could not remove."));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("getNodeType")]
        public async Task<ActionResult<List<Sys_Environment>>> GetNodeType()
        {
            try
            {
                var list = await _integrationAPIService.GetNodeType();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("checkEndPoint")]
        public async Task<ActionResult> checkDomainSystemHealth(string url)
        {
            try
            {
                using WebClient client = new WebClient();
                string s1 = client.DownloadString(url);
                return Ok(new ApiSuccessResult<string>());
            }
            catch (Exception ex)
            {
                return Ok(new ApiErrorResult<string>("End-point un validated"));
            }
        }
    }
}
