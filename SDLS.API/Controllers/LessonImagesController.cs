using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs.LessonImage;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    public class LessonImageUploadRequest
    {
        public Guid QuestionLessonId { get; set; }
        public string? Name { get; set; }
        public IFormFile File { get; set; } = null!;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class LessonImagesController : ControllerBase
    {
        private readonly ILessonImageService _service;

        public LessonImagesController(ILessonImageService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonImageDTO>>> GetAll()
        {
            var images = await _service.GetAllAsync();
            return Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LessonImageDTO>> GetById(Guid id)
        {
            try
            {
                var image = await _service.GetByIdAsync(id);
                return Ok(image);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("lesson/{lessonId}")]
        public async Task<ActionResult<LessonImageDTO>> GetByLessonId(Guid lessonId)
        {
            try
            {
                var image = await _service.GetByLessonIdAsync(lessonId);
                return Ok(image);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<LessonImageDTO>> Create([FromForm] LessonImageUploadRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _service.CreateAsync(request.File, request.QuestionLessonId, request.Name);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
