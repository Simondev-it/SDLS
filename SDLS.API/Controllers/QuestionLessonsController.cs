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

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<QuestionLessonDTO>> Create([FromBody] QuestionLessonCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpGet("template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            var template = await _service.GenerateImportTemplateAsync();
            return File(template.Content, template.ContentType, template.FileName);
        }

        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<List<QuestionLessonDTO>>> Import([FromForm] ImportQuestionLessonRequest request)
        {
            var imported = await _service.ImportAsync(request.File);
            return Ok(imported);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcel(
            [FromQuery] Guid? id,
            [FromQuery] Guid? questionChapterId,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] string? content,
            [FromQuery] int? status = null)
        {
            var file = await _service.ExportToExcelAsync(id, questionChapterId, name, description, content, status);
            return File(file.Content, file.ContentType, file.FileName);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionLessonDTO>> Update(Guid id, [FromBody] QuestionLessonUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<QuestionLessonDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<QuestionLessonDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}