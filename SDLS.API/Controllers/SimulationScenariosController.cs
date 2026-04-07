using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationScenario;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationScenariosController : ControllerBase
    {
        private readonly ISimulationScenarioService _service;

        public SimulationScenariosController(ISimulationScenarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<SimulationScenarioDTO>>> GetAll(
            [FromQuery] Guid? simulationCategoryId,
            [FromQuery] Guid? simulationChapterId,
            [FromQuery] Guid? simulationDifficultyLevelId,
            [FromQuery] string? name,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(
                simulationCategoryId,
                simulationChapterId,
                simulationDifficultyLevelId,
                name,
                status,
                page,
                pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SimulationScenarioDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<ActionResult<SimulationScenarioDTO>> Create([FromBody] SimulationScenarioCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPost("bulk")]
        public async Task<ActionResult<List<SimulationScenarioDTO>>> CreateMany([FromBody] List<SimulationScenarioCreateDTO> dtos)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dtos == null || dtos.Count == 0)
                return BadRequest("Danh sách tình hu?ng mô ph?ng không ???c r?ng.");

            var created = await _service.CreateManyAsync(dtos);
            return Ok(created);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<SimulationScenarioDTO>> Update(Guid id, [FromBody] SimulationScenarioUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Instructor")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<SimulationScenarioDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Instructor")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<SimulationScenarioDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}