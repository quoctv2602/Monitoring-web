using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring.Service.Services;
using Monitoring_Web.Filter;
using static Monitoring_Common.Enum;

namespace Monitoring_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : BaseController
    {
        private readonly ILogger<GroupController> _logger;
        private readonly IGroupService _groupService;
        public GroupController(ILogger<GroupController> logger, IGroupService groupService)
        {
            _logger = logger;
            _groupService = groupService;
        }
        [HttpPost]
        [Route("GetGroups")]
        public async Task<ActionResult<List<GroupModel>>> GetGroups(GroupFilterRequestModel filterModel)
        {
            try
            {
                _logger.LogInformation("---Begin method GetGroups in GroupController---");
                var listGroup = await _groupService.GetGroups(filterModel);
                _logger.LogInformation("---End method GetGroups in GroupController--");
                return Ok(listGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.userManage)]
        [HttpGet]
        [Route("ById")]
        public async Task<ActionResult<UserProfileModel>> GetUserProfile(int id)
        {
            try
            {
                _logger.LogInformation("---Begin method GetAll in GroupController---");
                var group = await _groupService.GetById(id);
                _logger.LogInformation("---End method GetAll in GroupController--");
                return Ok(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId =(int)ActionEnum.userPermissionManageGroup)]
        [HttpPost]
        [Route("Save")]
        public async Task<ActionResult<int>> SaveGroup([FromBody] GroupModel SaveModel)
        {
            try
            {
                _logger.LogInformation("---Begin method SaveGroup in GroupController---");
                var saveResult = await _groupService.SaveGroup(SaveModel);
                _logger.LogInformation("---End method SaveGroup in GroupController--");
                return Ok(saveResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.userPermissionSetDefaultGroup)]
        [HttpPost]
        [Route("ChangeDefault")]
        public async Task<ActionResult<int>> ChangeDefaultGroup([FromBody] GroupModel changeModel)
        {
            try
            {
                _logger.LogInformation("---Begin method ChangeDefaultGroup in GroupController---");
                var saveResult = await _groupService.ChangeDefault(changeModel);
                _logger.LogInformation("---End method ChangeDefaultGroup in GroupController--");
                return Ok(saveResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.userPermissionManageGroup)]
        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult<int>> Delete([FromBody] List<GroupModel> deleteModels)
        {
            try
            {
                _logger.LogInformation("---Begin method Delete in GroupController---");
                var saveResult = await _groupService.Delete(deleteModels);
                _logger.LogInformation("---End method Delete in GroupController--");
                return Ok(saveResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
