using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Monitoring.Service.IService;
using Monitoring_Common.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Web.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using IAuthorizationFilter = Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter;

namespace Monitoring_Web.Filter
{
    public class BaseAuthenticationAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var _distributedCache = context.HttpContext.RequestServices.GetService(typeof(IDistributedCache)) as IDistributedCache;
            var token = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            var email = tokenS?.Claims.First(claim => claim.Type == "email").Value;
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email))
            {
                var encryptString = CommonSetting.EncryptEmail(email);
                //_distributedCache?.Refresh(encryptString);
                var sessionJson = _distributedCache?.GetString(encryptString);
                if (sessionJson == null)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            return;
        }
    }
}
