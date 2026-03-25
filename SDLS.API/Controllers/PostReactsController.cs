using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.PostReact;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostReactsController : ControllerBase
    {
        private readonly IPostReactService _service;

        public PostReactsController(IPostReactService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<PostReactDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? forumPostId,
            [FromQuery] string? reactType,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetAllAsync(id, userId, forumPostId, reactType, status);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PostReactDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? forumPostId,
            [FromQuery] string? reactType,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(id, userId, forumPostId, reactType, status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostReactDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] PostReactCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] PostReactUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return Ok(true);
        }

        //[Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _service.DeleteSoftAsync(id);
            return NoContent();
        }

        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            await _service.DeleteHardAsync(id);
            return NoContent();
        }
    }
}