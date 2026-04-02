using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs.User;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            return Ok(await _auth.Login(email, password));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            return Ok(await _auth.Refresh(refreshToken));
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
        {
            var result = await _auth.Register(dto);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Đã xác thực thành công");
        }
    }
}
