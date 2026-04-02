using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.UserLicense;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserLicensesController : ControllerBase
    {
        private readonly IUserLicenseService _service;

        public UserLicensesController(IUserLicenseService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<ActionResult<List<UserLicenseDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? drivingLicenseId,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetAllAsync(id, userId, drivingLicenseId, status);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResult<UserLicenseDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] Guid? drivingLicenseId,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(id, userId, drivingLicenseId, status, page, pageSize);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserLicenseDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] UserLicenseCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] UserLicenseUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();

            return Ok(true);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _service.DeleteSoftAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> HardDelete(Guid id)
        {
            await _service.DeleteHardAsync(id);
            return NoContent();
        }
    }
}