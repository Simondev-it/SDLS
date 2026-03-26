using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionLesson;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionLessonsController : ControllerBase
    {
        private readonly IQuestionLessonService _service;

        public QuestionLessonsController(IQuestionLessonService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<QuestionLessonDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? questionChapterId,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] string? content,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(
                id: id,
                questionChapterId: questionChapterId,
                name: name,
                description: description,
                content: content,
                status: status,
                page: page,
                pageSize: pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionLessonDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] QuestionLessonCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] QuestionLessonUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _service.DeleteSoftAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            await _service.DeleteHardAsync(id);
            return NoContent();
        }
    }
}