using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ExamSession;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamSessionsController : ControllerBase
    {
        private readonly IExamSessionService _service;

        public ExamSessionsController(IExamSessionService service)
        {
            _service = service;
        }

        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResult<ExamSessionDTO>>> GetAll(
            [FromQuery] Guid? examId,
            [FromQuery] Guid? userId,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(examId, userId, status, page, pageSize);
            return Ok(result);
        }

        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamSessionDTO>> GetById(Guid id)
        {
            var examSession = await _service.GetByIdAsync(id);
            if (examSession == null) return NotFound();
            return Ok(examSession);
        }

        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] ExamSessionCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] ExamSessionUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        //[Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _service.DeleteSoftAsync(id);
            return NoContent();
        }

        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            await _service.DeleteHardAsync(id);
            return NoContent();
        }
    }
}