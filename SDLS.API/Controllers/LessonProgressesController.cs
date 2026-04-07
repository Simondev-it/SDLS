using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.LessonProgress;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonProgressesController : ControllerBase
    {
        private readonly ILessonProgressService _service;

        public LessonProgressesController(ILessonProgressService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<LessonProgressDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? questionLessonId,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetAllAsync(id, userId, questionLessonId, status);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<LessonProgressDTO>>> GetByUserId(
            Guid userId,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetByUserIdAsync(userId, status);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResult<LessonProgressDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? questionLessonId,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(id, userId, questionLessonId, status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LessonProgressDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<LessonProgressDTO>> Create([FromBody] LessonProgressCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<LessonProgressDTO>> Update(Guid id, [FromBody] LessonProgressUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<LessonProgressDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<LessonProgressDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}