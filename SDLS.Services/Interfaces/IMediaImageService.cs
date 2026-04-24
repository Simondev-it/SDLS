using SDLS.Model.DTOs.Media;
using Microsoft.AspNetCore.Http;

namespace SDLS.Services.Interfaces;

public interface IMediaImageService
{
    Task<List<MediaUploadResponseDTO>> UploadAsync(
        List<IFormFile> files,
        Guid entityId,
        string imageTarget);

    Task<bool> DeleteAsync(string fileUrl, string imageTarget);
    Task<string> UploadVideoAsync(IFormFile file);
    Task<bool> DeleteVideoAsync(string fileUrl);
}
