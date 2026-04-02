using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
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

        [Authorize]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<LearningProgressDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? questionId,
            [FromQuery] int? status = null)
        {
            var items = await _service.GetAllAsync(id, userId, questionId, status);
            return Ok(items);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResult<LearningProgressDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? questionId,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var items = await _service.GetPagedAsync(id, userId, questionId, status, page, pageSize);
            return Ok(items);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<LearningProgressDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize]
        [HttpGet("user-question")]
        public async Task<ActionResult<List<LearningProgressDTO>>> GetByUserAndQuestion(
            [FromQuery] Guid? userId,
            [FromQuery] Guid? questionId,
            [FromQuery] int? status = null)
        {
            if (!userId.HasValue && !questionId.HasValue)
                return BadRequest("Cần cung cấp ít nhất một trong hai tham số: userId hoặc questionId");

            var items = await _service.GetByUserAndQuestionAsync(userId, questionId, status);

            if (!items.Any())
                return NotFound("Không tìm thấy LearningProgress nào phù hợp");

            return Ok(items);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] LearningProgressCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] LearningProgressUpdateDTO dto)
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
