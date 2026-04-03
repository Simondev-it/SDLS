using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Question;
using SDLS.Model.Models;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _service;

        public QuestionsController(IQuestionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<QuestionDTO>>> GetAll(
            [FromQuery] Guid? lessonId,
            [FromQuery] Guid? topicId,
            [FromQuery] Guid? QuestionCategoryId,
            [FromQuery] List<Guid>? tagIds,
            [FromQuery] string? searchContent,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(
                lessonId, topicId, QuestionCategoryId, tagIds, searchContent, status, page, pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDTO>> GetById(Guid id)
        {
            var question = await _service.GetByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        //[Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<ActionResult<QuestionDTO>> Create([FromBody] QuestionCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        //[Authorize(Roles = "Instructor")]
        [HttpGet("import-template")]
        public async Task<IActionResult> DownloadImportTemplate([FromQuery] string format = "xlsx")
        {
            var content = await _service.DownloadImportTemplateAsync(format);
            var isCsv = string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase);
            var fileName = isCsv ? "question-import-template.csv" : "question-import-template.xlsx";
            var contentType = isCsv ? "text/csv" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(content, contentType, fileName);
        }

        //[Authorize(Roles = "Instructor")]
        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<QuestionImportResultDTO>> ImportQuestions([FromForm] QuestionImportFileDTO request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("File không hợp lệ.");

            var result = await _service.ImportQuestionsAsync(request.File);
            return Ok(result);
        }

        //[Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionDTO>> Update(Guid id, [FromBody] QuestionUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
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
