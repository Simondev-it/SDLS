using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ForumComment;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForumCommentsController : ControllerBase
    {
        private readonly IForumCommentService _service;

        public ForumCommentsController(IForumCommentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ForumCommentDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? forumPostId,
            [FromQuery] Guid? userId,
            [FromQuery] string? content,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(id, forumPostId, userId, content, status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ForumCommentDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ForumCommentDTO>> Create([FromBody] ForumCommentCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ForumCommentDTO>> Update(Guid id, [FromBody] ForumCommentUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<ForumCommentDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ForumCommentDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}
