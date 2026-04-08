using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SituationExam;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SituationExamsController : ControllerBase
    {
        private readonly ISituationExamService _service;

        public SituationExamsController(ISituationExamService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<SituationExamDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] string? title,
            [FromQuery] string? description,
            [FromQuery] bool? isRandom,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(id, title, description, isRandom, status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SituationExamDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<ActionResult<SituationExamDTO>> Create([FromBody] SituationExamCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<SituationExamDTO>> Update(Guid id, [FromBody] SituationExamUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<SituationExamDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Instructor")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<SituationExamDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}