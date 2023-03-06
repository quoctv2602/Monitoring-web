using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Monitoring_Web.Filter;

namespace Monitoring_Web.Controllers
{
    [BaseAuthentication]
    public class BaseController : ControllerBase
    {
    }
}
