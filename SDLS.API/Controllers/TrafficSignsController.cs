using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.TrafficSign;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrafficSignsController : ControllerBase
    {
        private readonly ITrafficSignService _service;

        public TrafficSignsController(ITrafficSignService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<TrafficSignDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? signCategoryId,
            [FromQuery] string? name,
            [FromQuery] string? code,
            [FromQuery] string? description,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(
                id, signCategoryId, name, code, description, status, page, pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrafficSignDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<ActionResult<TrafficSignDTO>> Create([FromBody] TrafficSignCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost("bulk")]
        public async Task<ActionResult<bool>> CreateMany([FromBody] List<TrafficSignCreateDTO> dtos)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dtos == null || dtos.Count == 0)
                return BadRequest("Danh sách bi?n báo không ???c r?ng.");

            var created = await _service.CreateManyAsync(dtos);
            return Ok(created);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<TrafficSignDTO>> Update(Guid id, [FromBody] TrafficSignUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<TrafficSignDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Instructor")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<TrafficSignDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}