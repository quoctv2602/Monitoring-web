using HealthMeasurement.Api.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Monitoring_Common.Security;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HealthMeasurement.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

    public class SecretKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.HttpContext.Request.Headers.Authorization.Count == 0)
            {
                var apiErrorResult = new ApiErrorResult<string>("Token is not Invalid!");
                context.Result = new BadRequestObjectResult(apiErrorResult);
                return;
            }
            var header = AuthenticationHeaderValue.Parse(context.HttpContext.Request.Headers["Authorization"]);
            string _potentialtoken = header.Parameter;
            if (string.IsNullOrEmpty(_potentialtoken))
            {
                var apiErrorResult = new ApiErrorResult<string>("Token is not Invalid!");
                context.Result = new BadRequestObjectResult(apiErrorResult);
                return;
            }
            else
            {
                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var Appid = MyConfig.GetValue<string>("MyConfig:Appid");
                var privateKey = MyConfig.GetValue<string>("MyConfig:HealthMeasurementKey");
              //  var dataEncryptAES = EncryptDecryptAES.EncryptAES(Appid, privateKey);

                var dataTokenDecrypt = AES.DecryptAES(_potentialtoken, privateKey);
                if (dataTokenDecrypt != Appid || dataTokenDecrypt == null)
                {
                    var apiErrorResult = new  ApiErrorResult<string>("Token is not Invalid!");
                    context.Result = new BadRequestObjectResult(apiErrorResult);
                    return ;
                }
            }
            await next();
        }

    }
}
