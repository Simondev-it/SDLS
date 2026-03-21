using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs.LearningProgress;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearningProgressesController : ControllerBase
    {
        private readonly ILearningProgressService _service;

        public LearningProgressesController(ILearningProgressService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LearningProgressDTO>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LearningProgressDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("user-question")]
        public async Task<ActionResult<List<LearningProgressDTO>>> GetByUserAndQuestion(
            [FromQuery] Guid? userId,
            [FromQuery] Guid? questionId)
        {
            if (!userId.HasValue && !questionId.HasValue)
                return BadRequest("Cần cung cấp ít nhất một trong hai tham số: userId hoặc questionId");

            var items = await _service.GetByUserAndQuestionAsync(userId, questionId);

            if (!items.Any())
                return NotFound("Không tìm thấy LearningProgress nào phù hợp");

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<LearningProgressDTO>> Create([FromBody] LearningProgressCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LearningProgressDTO>> Update(Guid id, [FromBody] LearningProgressUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
