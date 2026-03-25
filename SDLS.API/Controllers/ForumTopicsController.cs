using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ForumTopic;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForumTopicsController : ControllerBase
    {
        private readonly IForumTopicService _service;

        public ForumTopicsController(IForumTopicService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<ForumTopicDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetAllAsync(id, name, description, status);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ForumTopicDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(id, name, description, status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ForumTopicDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        //[Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] ForumTopicCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        //[Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] ForumTopicUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        //[Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _service.DeleteSoftAsync(id);
            return NoContent();
        }

        //[Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            await _service.DeleteHardAsync(id);
            return NoContent();
        }
    }
}