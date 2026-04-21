using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SystemConfig;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemConfigsController : ControllerBase
    {
        private readonly ISystemConfigService _service;

        public SystemConfigsController(ISystemConfigService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<SystemConfigDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] int? value,
            [FromQuery] string? description,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetAllAsync(id, name, value, description, status);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<SystemConfigDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] int? value,
            [FromQuery] string? description,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(id, name, value, description, status, page, pageSize);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard-summary")]
        public async Task<ActionResult<SystemDashboardSummaryDTO>> GetDashboardSummary()
        {
            var result = await _service.GetDashboardSummaryAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpGet("dashboard-summary/instructor")]
        public async Task<ActionResult<InstructorDashboardSummaryDTO>> GetInstructorDashboardSummary()
        {
            var result = await _service.GetInstructorDashboardSummaryAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard-summary/export")]
        public async Task<IActionResult> ExportDashboardSummary()
        {
            var file = await _service.ExportDashboardSummaryExcelAsync();
            return File(file.Content, file.ContentType, file.FileName);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SystemConfigDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<SystemConfigDTO>> Create([FromBody] SystemConfigCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<SystemConfigDTO>> Update(Guid id, [FromBody] SystemConfigUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<SystemConfigDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<SystemConfigDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}
