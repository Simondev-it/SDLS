using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Notification;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResult<NotificationDTO>>> GetAll(
            [FromQuery] Guid? userId,
            [FromQuery] string? title,
            [FromQuery] string? content,
            [FromQuery] int? status = null,
            [FromQuery] string? sortBy = "time",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(userId, title, content, status, sortBy, page, pageSize);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] NotificationCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] NotificationUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return Ok(true);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _service.DeleteSoftAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            await _service.DeleteHardAsync(id);
            return NoContent();
        }
    }
}