using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Question;
using SDLS.Model.Models;
using SDLS.Services.Interfaces;
using SDLS.Services.Services;

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
            [FromQuery] string? sortBy,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(
                lessonId, topicId, QuestionCategoryId, tagIds, searchContent, status, page, pageSize, sortBy);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDTO>> GetById(Guid id)
        {
            var question = await _service.GetByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/{id}")]
        // [Authorize(Roles = RoleConst.ADMIN_ROLE_NAME)] // Thêm phân quyền nếu cần
        public async Task<IActionResult> GetByIdForAdmin(Guid id)
        {
            var result = await _service.GetByIdForAdminAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPost]
        public async Task<ActionResult<QuestionDTO>> Create([FromBody] QuestionCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPost("bulk")]
        public async Task<ActionResult<List<QuestionDTO>>> CreateMany([FromBody] List<QuestionCreateDTO> dtos)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dtos == null || dtos.Count == 0)
                return BadRequest("Danh sách câu hỏi không được rỗng.");

            var created = await _service.CreateManyAsync(dtos);
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
        public async Task<ActionResult<List<QuestionDTO>>> Import([FromForm] ImportQuestionRequest request)
        {
            var imported = await _service.ImportAsync(request.File);
            return Ok(imported);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionDTO>> Update(Guid id, [FromBody] QuestionUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<QuestionDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<QuestionDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}
