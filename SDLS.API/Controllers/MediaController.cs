using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs.Media;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaImageService _service;

    public MediaController(IMediaImageService service)
    {
        _service = service;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<List<MediaUploadResponseDTO>>> Upload([FromForm] MediaUploadRequestDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _service.UploadAsync(
                request.Files,
                request.EntityId,
                request.ImageTarget);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string fileUrl, [FromQuery] string imageTarget)
    {
        try
        {
            await _service.DeleteAsync(fileUrl, imageTarget);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
