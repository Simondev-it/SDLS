using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Tag;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _service;

        public TagsController(ITagService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<TagDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] string? colorCode,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetAllAsync(id, name, description, colorCode, status);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<TagDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] string? colorCode,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(id, name, description, colorCode, status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TagDTO>> Create([FromBody] TagCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            var template = await _service.GenerateImportTemplateAsync();
            return File(template.Content, template.ContentType, template.FileName);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<List<TagDTO>>> Import([FromForm] ImportTagRequest request)
        {
            var imported = await _service.ImportAsync(request.File);
            return Ok(imported);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<TagDTO>> Update(Guid id, [FromBody] TagUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<TagDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<TagDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}