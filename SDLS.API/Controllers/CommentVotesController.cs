using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.CommentVote;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentVotesController : ControllerBase
    {
        private readonly ICommentVoteService _service;

        public CommentVotesController(ICommentVoteService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<CommentVoteDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? forumCommentId,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetAllAsync(id, userId, forumCommentId, status);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<CommentVoteDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? forumCommentId,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(id, userId, forumCommentId, status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentVoteDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CommentVoteDTO>> Create([FromBody] CommentVoteCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<CommentVoteDTO>> Update(Guid id, [FromBody] CommentVoteUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize]
        [HttpPatch("{id}/soft-delete")]
        public async Task<ActionResult<CommentVoteDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<CommentVoteDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}