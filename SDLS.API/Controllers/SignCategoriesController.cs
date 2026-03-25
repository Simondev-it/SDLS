using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SignCategory;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignCategoriesController : ControllerBase
    {
        private readonly ISignCategoryService _service;

        public SignCategoriesController(ISignCategoryService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<SignCategoryDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description)
        {
            var result = await _service.GetAllAsync(id, name, description);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<SignCategoryDTO>>> GetPaged(
            [FromQuery] Guid? id,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(id, name, description, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SignCategoryDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] SignCategoryCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] SignCategoryUpdateDTO dto)
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