using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs.ForumPost;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{
    public class PostImageUploadRequest
    {
        public Guid ForumPostId { get; set; }
        public string? Name { get; set; }
        public IFormFile File { get; set; } = null!;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class PostImagesController : ControllerBase
    {
        private readonly IPostImageService _service;

        public PostImagesController(IPostImageService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForumPostImageDTO>>> GetAll()
        {
            var images = await _service.GetAllAsync();
            return Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ForumPostImageDTO>> GetById(Guid id)
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

        [HttpGet("post/{postId}")]
        public async Task<ActionResult<IEnumerable<ForumPostImageDTO>>> GetByPostId(Guid postId)
        {
            try
            {
                var images = await _service.GetByPostIdAsync(postId);
                return Ok(images);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ForumPostImageDTO>> Create([FromForm] PostImageUploadRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _service.CreateAsync(request.File, request.ForumPostId, request.Name);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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