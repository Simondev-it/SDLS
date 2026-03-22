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
            [FromQuery] int? status = 1,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(
                id,
                forumTopicId,
                userId,
                name,
                title,
                content,
                status,
                page,
                pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ForumPostDTO>> GetById(Guid id)
        {
            var forumPost = await _service.GetByIdAsync(id);
            if (forumPost == null)
                return NotFound();

            return Ok(forumPost);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] ForumPostCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] ForumPostUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
