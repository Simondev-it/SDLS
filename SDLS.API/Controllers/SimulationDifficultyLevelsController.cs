using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationDifficultyLevel;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationDifficultyLevelsController : ControllerBase
    {
        private readonly ISimulationDifficultyLevelService _service;

        public SimulationDifficultyLevelsController(ISimulationDifficultyLevelService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<SimulationDifficultyLevelDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetAllAsync(id, name, description, status);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<SimulationDifficultyLevelDTO>>> GetPaged(
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
        public async Task<ActionResult<SimulationDifficultyLevelDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpPost]
        public async Task<ActionResult<SimulationDifficultyLevelDTO>> Create([FromBody] SimulationDifficultyLevelCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<SimulationDifficultyLevelDTO>> Update(Guid id, [FromBody] SimulationDifficultyLevelUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<SimulationDifficultyLevelDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<SimulationDifficultyLevelDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}