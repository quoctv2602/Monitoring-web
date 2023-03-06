using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring.Service.Services;
using Monitoring_Common.Common;
using Monitoring_Common.Security;
using Monitoring_Web.Filter;
using Monitoring_Web.HubConfig;
using Monitoring_Web.TimerFeatures;
using Newtonsoft.Json;
using System.Drawing.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Monitoring_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAccountService _accountService;
        private readonly IDistributedCache _distributedCache;
        public AccountController(ILogger<AccountController> logger, IConfiguration configuration, IAccountService accountService, IDistributedCache distributedCache)
        {
            _logger = logger;
            _configuration = configuration;
            _accountService = accountService;
            _distributedCache = distributedCache;
        }
        [Route("OnLoginSuccess")]
        [HttpGet]
        public async Task<ActionResult<UserLoginModel>> OnLoginSuccess()
        {

            try
            {
                _logger.LogInformation("---Begin method OnLoginSuccess in AccountController---");
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var tokenS = handler.ReadToken(token) as JwtSecurityToken;
                var email = tokenS?.Claims.First(claim => claim.Type == "email").Value;
                var audience = tokenS?.Audiences.FirstOrDefault();
                //var cachedPostsJson = await _distributedCache.GetStringAsync("Posts");
                if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(audience) || string.IsNullOrWhiteSpace(audience))
                {
                    return Unauthorized();
                }
                var configIDP = _configuration.GetSection("IDP");
                var appId = configIDP["ClientId"];
                var returnModel = new UserLoginModel();
                if (audience != appId)
                {
                    return Unauthorized();
                }
                else
                {
                    returnModel = await _accountService.OnLoginSuccess(email);
                    var sessionTokenModel = new SessionTokenModel();
                    sessionTokenModel.Token = token;
                    sessionTokenModel.Permissions = returnModel.Permission;
                    string sessionTokenString = JsonConvert.SerializeObject(sessionTokenModel);
                    var encryptString = CommonSetting.EncryptEmail(email);
                    await _distributedCache.SetStringAsync(encryptString, sessionTokenString);
                }
                _logger.LogInformation("---End method OnLoginSuccess in AccountController---");
                return Ok(returnModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [Route("LogOut")]
        [HttpGet]
        public async Task<ActionResult<int>> OnLogOut()
        {
            try
            {
                //var base64EncodedBytes = System.Convert.FromBase64String("SG9hLlYuTmd1eWVuQHRydWVjb21tZXJjZS5jb20=");
                //return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                _logger.LogInformation("---Begin method OnLogOut in AccountController---");
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var tokenS = handler.ReadToken(token) as JwtSecurityToken;
                var email = tokenS?.Claims.First(claim => claim.Type == "email").Value;
                var audience = tokenS?.Audiences.FirstOrDefault();
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email))
                {
                    var encryptString = CommonSetting.EncryptEmail(email);
                    await _distributedCache.RemoveAsync(encryptString);
                    return 1;
                }
                _logger.LogInformation("---End method OnLogOut in AccountController---");
                return Ok(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter()]
        [Route("ConfigurationData")]
        [HttpGet]
        public IActionResult ConfigurationData()
        {
            try
            {
                _logger.LogInformation("---Begin method ConfigurationData in AccountController---");
                _logger.LogInformation("---End method ConfigurationData in AccountController---");
                return Ok(_configuration.GetSection("IDP").GetChildren().ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}
