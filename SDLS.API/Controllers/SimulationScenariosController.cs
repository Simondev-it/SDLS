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
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(
                simulationCategoryId,
                simulationChapterId,
                simulationDifficultyLevelId,
                name,
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

        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] SimulationScenarioCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] SimulationScenarioUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}