using Microsoft.AspNetCore.Mvc;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using static Monitoring_Common.Enum;
using System;
using Monitoring_Web.Filter;

namespace Monitoring_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : BaseController
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IPermissionService _permissionService;
        public PermissionController(ILogger<PermissionController> logger, IPermissionService permissionService)
        {
            _logger = logger;
            _permissionService = permissionService;
        }
        [HttpGet]
        [Route("PermissionByGroup")]
        public async Task<ActionResult<List<PermissionModel>>> GetPermissionByGroup(int groupId)
        {
            try
            {
                _logger.LogInformation("---Begin method GetPermissionByGroup in PermissionController---");
                var list = await _permissionService.GetByGroup(groupId);
                _logger.LogInformation("---End method GetPermissionByGroup in PermissionController---");
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.notificationSettingsManage)]
        [HttpPost]
        [Route("SavePermission")]
        public async Task<ActionResult<int>> SavePermission([FromBody] SavePermissionModel saveModel)
        {
            try
            {
                _logger.LogInformation("---Begin method SavePermission in DashboardController---");
                var rowsAffect = await _permissionService.SavePermission(saveModel.GroupId,saveModel.Permissions);
                _logger.LogInformation("---End method SavePermission in DashboardController---");
                return Ok(rowsAffect);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
