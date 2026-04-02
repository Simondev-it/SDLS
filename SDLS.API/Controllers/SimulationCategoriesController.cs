using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationCategory;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationCategoriesController : ControllerBase
    {
        private readonly ISimulationCategoryService _service;

        public SimulationCategoriesController(ISimulationCategoryService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<SimulationCategoryDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetAllAsync(id, name, description, status);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<SimulationCategoryDTO>>> GetPaged(
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
        public async Task<ActionResult<SimulationCategoryDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] SimulationCategoryCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] SimulationCategoryUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _service.DeleteSoftAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            await _service.DeleteHardAsync(id);
            return NoContent();
        }
    }
}