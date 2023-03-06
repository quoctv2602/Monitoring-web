
using HealthMeasurement.Api.Common;
using HealthMeasurement.Api.Constants;
using HealthMeasurement.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Monitoring_Common.Security;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthMeasurement.Api.Service
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;

        public UserService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<ApiResult<string>> Authencate(LoginRequest request)
        {
            try
            {
                var privateKey = _config[GlobalSettings.HealthMeasurementKey()];
                var Appid = _config[GlobalSettings.HealthMeasurementKey()];
                var dataEncryptAES = AES.EncryptAES(Appid, privateKey);

                var dataTokenDecrypt = AES.DecryptAES(dataEncryptAES, privateKey);
                if (dataTokenDecrypt != Appid || dataTokenDecrypt == null)
                {
                    return new ApiErrorResult<string>("Incorrect login");
                }

                var claims = new[]
                {
                new Claim(ClaimTypes.Email,SystemConstants.Email),
                new Claim(ClaimTypes.GivenName,SystemConstants.UserName),
                new Claim(ClaimTypes.Name, SystemConstants.NormalizedUserName)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                    _config["Tokens:Issuer"],
                    claims,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: creds);

                return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<string>("Incorrect login");
            }
        }
    }
}
