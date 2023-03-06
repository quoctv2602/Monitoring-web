using HealthMeasurement.Api.Common;
using HealthMeasurement.Model;
using System.Threading.Tasks;

namespace HealthMeasurement.Api.Service
{
    public interface IUserService
    {
        Task<ApiResult<string>> Authencate(LoginRequest request);

    }
}
