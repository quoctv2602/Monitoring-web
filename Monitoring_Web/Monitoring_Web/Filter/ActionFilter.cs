using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Monitoring.Model.Model;
using Monitoring_Common.Common;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace Monitoring_Web.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ActionFilter : ActionFilterAttribute, IActionFilter
    {
        public int ActionId { get; set; }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var _distributeCache = context.HttpContext.RequestServices.GetService(typeof(IDistributedCache)) as IDistributedCache;
                var header = AuthenticationHeaderValue.Parse(context.HttpContext.Request.Headers["Authorization"]);
                string token = header.Parameter??"";
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var tokenS = handler.ReadToken(token) as JwtSecurityToken;
                var email = tokenS?.Claims.First(claim => claim.Type == "email").Value;
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email))
                {
                    var encryptEmail = CommonSetting.EncryptEmail(email);
                    var sessionJson = _distributeCache?.GetString(encryptEmail);
                    var session = JsonConvert.DeserializeObject<SessionTokenModel>(sessionJson ?? "");
                    if (session == null)
                    {
                        context.Result = new UnauthorizedResult();
                    }
                    else
                    {
                        var permissions = session.Permissions??new List<int>();
                        if (permissions.Contains(ActionId) == false)
                        {
                            context.Result = new ForbidResult();
                        }
                    }
                }
            }
            catch
            {
                throw new NotImplementedException();

            }
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {

            }
            catch
            {
                throw new NotImplementedException();
            }
        }
    }
}
