using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring.Service.Services;

namespace Monitoring_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : BaseController
    {
        private readonly ILogger<UserProfileController> _logger;
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(ILogger<UserProfileController> logger, IUserProfileService userProfileService)
        {
            _logger = logger;
            _userProfileService = userProfileService;
        }
        [HttpPost]
        [Route("UserList")]
        public async Task<ActionResult<List<UserProfileModel>>> GetUsers([FromBody] UserProfileFilterRequestModel filterMode)
        {
            try
            {
                _logger.LogInformation("---Begin method GetUsers in UserProfileController---");
                var listUserProfile = await _userProfileService.GetUsers(filterMode);
                _logger.LogInformation("---End method GetUsers in UserProfileController--");
                return Ok(listUserProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("ById")]
        public async Task<ActionResult<UserProfileModel>> GetUserProfile(int id)
        {
            try
            {
                _logger.LogInformation("---Begin method GetAll in UserProfileController---");
                var userProfile = await _userProfileService.GetUserProfile(id);
                _logger.LogInformation("---End method GetAll in UserProfileController--");
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("Save")]
        public async Task<ActionResult<int>> Save([FromBody] UserProfileModel saveModel)
        {
            try
            {
                _logger.LogInformation("---Begin method Save in UserProfileController---");
                var userProfile = await _userProfileService.Save(saveModel);
                _logger.LogInformation("---End method Save in UserProfileController--");
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult<int>> Delete([FromBody] UserProfileModel deleteModel)
        {
            try
            {
                _logger.LogInformation("---Begin method Delete in UserProfileController---");
                var result = await _userProfileService.Delete(deleteModel);
                _logger.LogInformation("---End method Delete in UserProfileController--");
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
