using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SDLS.Model.DTOs.Media;

public class VideoUploadRequestDTO
{
    [Required(ErrorMessage = "Video là bắt buộc.")]
    public IFormFile File { get; set; } = null!;
}