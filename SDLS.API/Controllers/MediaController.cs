using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SDLS.Model.DTOs.Media;
using SDLS.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace SDLS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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

        var result = await _service.UploadAsync(
            request.Files,
            request.EntityId,
            request.ImageTarget);

        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string fileUrl, [FromQuery] string imageTarget)
    {
        await _service.DeleteAsync(fileUrl, imageTarget);
        return NoContent();
    }

    [HttpPost("video")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(VideoUploadResponseDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<VideoUploadResponseDTO>> UploadVideo([FromForm] VideoUploadRequestDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var url = await _service.UploadVideoAsync(request.File);
        return Ok(new VideoUploadResponseDTO { Url = url });
    }

    [HttpDelete("video")]
    public async Task<IActionResult> DeleteVideo([FromQuery] string fileUrl)
    {
        await _service.DeleteVideoAsync(fileUrl);
        return NoContent();
    }
}
