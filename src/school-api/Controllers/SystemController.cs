using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Services.Interfaces;

namespace SchoolApi.Controllers
{
    [Route("api/v1/system")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "GlobalAdminOnly")]
    public class SystemController : ControllerBase
    {
        private readonly IChatService _chatService;

        public SystemController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("chats")]
        public async Task<IActionResult> CreateDefaultSchoolChats()
        {
            await _chatService.CreateDefaultChatsAsync();

            return Ok();
        }
    }
}