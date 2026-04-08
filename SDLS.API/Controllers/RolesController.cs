using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Role;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _service;

        public RolesController(IRoleService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        //[Authorize]
        [HttpGet("all")]
        public async Task<ActionResult<List<RoleDTO>>> GetList(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] int? status = null)
        {
            var result = await _service.GetListAsync(id, name, description, status);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResult<RoleDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(id, name, description, status, page, pageSize);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [Authorize(Roles = "Admin")]
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<RoleDTO>> Create([FromBody] RoleCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "Admin")]
        //[Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<RoleDTO>> Update(Guid id, [FromBody] RoleUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Admin")]
        //[Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<RoleDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize(Roles = "Admin")]
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<RoleDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}