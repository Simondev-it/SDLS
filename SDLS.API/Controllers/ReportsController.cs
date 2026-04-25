using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Report;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportsController(IReportService service)
        {
            _service = service;
        }

        //[Authorize(Roles = "Instructor")]
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResult<ReportDTO>>> GetAll(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] List<Guid>? reportCategoryIds,
            [FromQuery] string? roleName,
            [FromQuery] Guid? simulationId,
            [FromQuery] Guid? forumPostId,
            [FromQuery] Guid? forumCommentId,
            [FromQuery] Guid? questionId,
            [FromQuery] string? title,
            [FromQuery] string? content,
            [FromQuery] int? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllAsync(
                id, userId, reportCategoryIds, roleName, simulationId, forumPostId, forumCommentId, questionId, title, content, status, page, pageSize);

            return Ok(result);
        }

        //[Authorize(Roles = "Instructor")]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportDTO>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReportDTO>> Create([FromBody] ReportCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ReportDTO>> Update(Guid id, [FromBody] ReportUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPatch("{id}/approve")]
        public async Task<ActionResult<ReportDTO>> Approve(Guid id, [FromBody] ReportResolveActionDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.ApproveAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Instructor,Admin")]
        [HttpPatch("{id}/disapprove")]
        public async Task<ActionResult<ReportDTO>> Disapprove(Guid id, [FromBody] ReportResolveActionDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.DisapproveAsync(id, dto);
            return Ok(result);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<ReportDTO>> SoftDelete(Guid id)
        {
            var deleted = await _service.DeleteSoftAsync(id);
            return Ok(deleted);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReportDTO>> HardDelete(Guid id)
        {
            var deleted = await _service.DeleteHardAsync(id);
            return Ok(deleted);
        }
    }
}