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

        [Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] QuestionTopicCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] QuestionTopicUpdateDTO dto)
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