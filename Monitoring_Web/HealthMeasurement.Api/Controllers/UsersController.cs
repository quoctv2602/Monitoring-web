using System;
using System.Threading.Tasks;
using HealthMeasurement.Api.Service;
using HealthMeasurement.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMeasurement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Authencate(request);

            if (string.IsNullOrEmpty(""))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}