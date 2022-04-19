using System.Threading.Tasks;
using IdentityServer.Interfaces;
using LionwoodSoftware.DataFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    [Route("api/v1/system")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SystemController : ControllerBase
    {
        private readonly ISystemService _systemService;

        public SystemController(ISystemService systemService)
        {
            _systemService = systemService;
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetPrivateUserById(string id, bool withRoles = false, bool withImage = false)
        {
            var user = await _systemService.GetPrivateUserByIdAsync(id, withRoles, withImage);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetPrivateUsers([FromQuery] KendoDataFilter dataFilter, [FromQuery] bool withRoles = false, [FromQuery] bool withImage = false)
        {
            var user = await _systemService.GetPrivateUsersAsync(dataFilter, withRoles, withImage);

            return Ok(user);
        }
    }
}