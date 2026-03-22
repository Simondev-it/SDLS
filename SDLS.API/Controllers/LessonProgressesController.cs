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
            [FromQuery] Guid? questionLessonId)
        {
            var result = await _service.GetAllAsync(id, userId, questionLessonId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<LessonProgressDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? questionLessonId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(id, userId, questionLessonId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LessonProgressDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] LessonProgressCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] LessonProgressUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();

            return Ok(true);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}