using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ForumPost;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForumPostsController : ControllerBase
    {
        private readonly IForumPostService _service;

        public ForumPostsController(IForumPostService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ForumPostDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? forumTopicId,
            [FromQuery] Guid? userId,
            [FromQuery] string? name,
            [FromQuery] string? title,
            [FromQuery] string? content,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(id, forumTopicId, userId, name, title, content, status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ForumPostDTO>> GetById(Guid id)
        {
            var forumPost = await _service.GetByIdAsync(id);
            if (forumPost == null) return NotFound();
            return Ok(forumPost);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ForumPostDTO>> Create([FromBody] ForumPostCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPost("manager")]
        public async Task<ActionResult<ForumPostDTO>> CreateByInstructor([FromBody] ForumPostCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateByInstructorAsync(dto);
            return Ok(created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ForumPostDTO>> Update(Guid id, [FromBody] ForumPostUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPatch("{id}/approve")]
        public async Task<ActionResult<ForumPostDTO>> Approve(Guid id)
        {
            var result = await _service.ApproveAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPatch("{id}/disapprove")]
        public async Task<ActionResult<ForumPostDTO>> Disapprove(Guid id)
        {
            var result = await _service.DisapproveAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Instructor,Student,Admin")]
        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<ForumPostDTO>> ToggleStatus(Guid id)
        {
            var result = await _service.ToggleStatusAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPatch("{id}/toggle-pin")]
        public async Task<ActionResult<ForumPostDTO>> TogglePinStatus(Guid id)
        {
            var result = await _service.TogglePinStatusAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<ForumPostDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPatch("{id}/force-delete")]
        public async Task<ActionResult<ForumPostDTO>> ForceDelete(Guid id)
        {
            var deleted = await _service.ForceDeleteAsync(id);
            return Ok(deleted);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ForumPostDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}
