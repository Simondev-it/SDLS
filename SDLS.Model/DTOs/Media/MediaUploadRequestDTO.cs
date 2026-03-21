using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SDLS.Model.DTOs.Media;

public class MediaUploadRequestDTO
{
    [Required(ErrorMessage = "EntityId là bắt buộc")]
    public Guid EntityId { get; set; }

    [Required(ErrorMessage = "ImageTarget là bắt buộc")]
    public string ImageTarget { get; set; } = null!;

    public List<IFormFile> Files { get; set; } = new();
}
