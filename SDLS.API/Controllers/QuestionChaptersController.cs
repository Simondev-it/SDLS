using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionChapter;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionChaptersController : ControllerBase
    {
        private readonly IQuestionChapterService _service;

        public QuestionChaptersController(IQuestionChapterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<QuestionChapterDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? drivingLicenseId,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] int? status = 1,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(
                id, drivingLicenseId, name, description, status, page, pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionChapterDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        //[Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] QuestionChapterCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        //[Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] QuestionChapterUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        //[Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _service.DeleteSoftAsync(id);
            return NoContent();
        }

        //[Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            await _service.DeleteHardAsync(id);
            return NoContent();
        }
    }
}