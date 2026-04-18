using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.User;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/user
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? roleId,
            [FromQuery] string? email,
            [FromQuery] string? name,
            [FromQuery] int? status)
        {
            var users = await _userService.GetAllAsync(id, roleId, email, name, status);
            return Ok(users);
        }

        // GET: api/user/paged?page=1&pageSize=20
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<UserDTO>>> GetAllPaged(
            [FromQuery] Guid? id,
            [FromQuery] Guid? roleId,
            [FromQuery] string? email,
            [FromQuery] string? name,
            [FromQuery] int? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var users = await _userService.GetAllWithPagingAsync(id, roleId, email, name, status, page, pageSize);
            return Ok(users);
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        // GET: api/user/email?email=abc@gmail.com
        [HttpGet("email")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null) return NotFound();

            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDTO user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.CreateAsync(user);
            return Ok(result);
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDTO user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.UpdateAsync(id, user);
            if (result == null) return NotFound();

            return Ok(result);
        }

        // PATCH: api/user/change-password
        [Authorize]
        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userService.ChangePasswordCurrentUserAsync(dto);
            if (!result) return BadRequest();

            return Ok("Changed password successfully");
        }

        // PATCH: api/user/{id}/toggle-active
        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActiveStatus(Guid id)
        {
            var result = await _userService.ToggleActiveStatusAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        // PATCH: api/user/{id}/toggle-lock
        [HttpPatch("{id}/toggle-lock")]
        public async Task<IActionResult> ToggleLockStatus(Guid id)
        {
            var result = await _userService.ToggleLockStatusAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result) return NotFound();

            return Ok("Deleted successfully");
        }
    }
}
