using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SavedQuestion;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SavedQuestionsController : ControllerBase
    {
        private readonly ISavedQuestionService _service;

        public SavedQuestionsController(ISavedQuestionService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<ActionResult<List<SavedQuestionDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? questionId,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetAllAsync(id, userId, questionId, status);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResult<SavedQuestionDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? questionId,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(id, userId, questionId, status, page, pageSize);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<SavedQuestionDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SavedQuestionDTO>> Create([FromBody] SavedQuestionCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<SavedQuestionDTO>> Update(Guid id, [FromBody] SavedQuestionUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<SavedQuestionDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<SavedQuestionDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}