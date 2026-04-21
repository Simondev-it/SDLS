using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionTopic;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionTopicsController : ControllerBase
    {
        private readonly IQuestionTopicService _service;

        public QuestionTopicsController(IQuestionTopicService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<QuestionTopicDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetAllAsync(id, name, description, status);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<QuestionTopicDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(id, name, description, status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionTopicDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<QuestionTopicDTO>> Create([FromBody] QuestionTopicCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        //[Authorize(Roles = "Instructor,Admin")]
        [HttpGet("template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            var template = await _service.GenerateImportTemplateAsync();
            return File(template.Content, template.ContentType, template.FileName);
        }

        //[Authorize(Roles = "Instructor,Admin")]
        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<List<QuestionTopicDTO>>> Import([FromForm] ImportQuestionTopicRequest request)
        {
            var imported = await _service.ImportAsync(request.File);
            return Ok(imported);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcel(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] int? status = null)
        {
            var file = await _service.ExportToExcelAsync(id, name, description, status);
            return File(file.Content, file.ContentType, file.FileName);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionTopicDTO>> Update(Guid id, [FromBody] QuestionTopicUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<QuestionTopicDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<QuestionTopicDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}