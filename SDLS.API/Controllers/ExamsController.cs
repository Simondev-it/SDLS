using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Exam;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _service;

        public ExamsController(IExamService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ExamDTO>>> GetAll(
            [FromQuery] Guid? userId,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(userId, status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExamDTO>> GetById(Guid id)
        {
            var exam = await _service.GetByIdAsync(id);
            if (exam == null) return NotFound();
            return Ok(exam);
        }

        [Authorize(Roles = "Instructor,Student,Admin")]
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<ExamDTO>> Create([FromBody] ExamCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Instructor,Student,Admin")]
        [HttpPost("random")]
        public async Task<ActionResult<ExamDTO>> CreateRandom([FromBody] ExamRandomCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateRandomAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ExamDTO>> Update(Guid id, [FromBody] ExamUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<ExamDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Instructor,Admin")]
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ExamDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}