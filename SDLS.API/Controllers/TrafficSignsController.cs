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

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<bool>> Create([FromForm] TrafficSignCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromForm] TrafficSignUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _service.DeleteSoftAsync(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            await _service.DeleteHardAsync(id);
            return NoContent();
        }
    }
}