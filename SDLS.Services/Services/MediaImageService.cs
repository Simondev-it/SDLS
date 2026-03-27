using SDLS.Model.DTOs.Media;
using SDLS.Model.Enumerations;
using SDLS.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace SDLS.Services.Services;

public class MediaImageService : IMediaImageService
{
    private readonly IStorageService _storageService;

    private const long MaxFileSizeBytes = 3 * 1024 * 1024;   // 3MB
    private const long MaxTotalSizeBytes = 10 * 1024 * 1024; // 10MB

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp"
    };

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "image/webp"
    };

    public MediaImageService(
        IStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<List<MediaUploadResponseDTO>> UploadAsync(
        List<IFormFile> files,
        Guid entityId,
        string imageTarget)
    {
        if (files == null || files.Count == 0)
            throw new ArgumentException("Không có file nào được upload.");

        // Validate
        ValidateFiles(files);

        // Parse ImageTarget enum
        if (!Enum.TryParse<ImageTarget>(imageTarget, out var imageTargetEnum))
            throw new ArgumentException($"ImageTarget '{imageTarget}' không hợp lệ.");

        var responses = new List<MediaUploadResponseDTO>();

        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            if (file == null || file.Length == 0)
                continue;

            // Upload to storage
            var url = await _storageService.UploadImageAsync(file, imageTargetEnum, entityId);

            var key = ExtractStorageKey(url, imageTargetEnum);
            responses.Add(new MediaUploadResponseDTO
            {
                Url = url,
                Key = key,
                Name = Path.GetFileNameWithoutExtension(file.FileName),
                MimeType = file.ContentType,
                FileSize = file.Length
            });
        }

        return responses;
    }

    public async Task<bool> DeleteAsync(string fileUrl, string imageTarget)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("fileUrl là bắt buộc.");

        if (!Enum.TryParse<ImageTarget>(imageTarget, out var imageTargetEnum))
            throw new ArgumentException($"ImageTarget '{imageTarget}' không hợp lệ.");

        await _storageService.DeleteImageAsync(fileUrl, imageTargetEnum);

        return true;
    }

    private static string ExtractStorageKey(string url, ImageTarget target)
    {
        var folder = target switch
        {
            ImageTarget.UserAvatar => "avatars",
            ImageTarget.TrafficSign => "traffic-signs",
            ImageTarget.NotificationImage => "notifications",
            ImageTarget.QuestionImage => "questions",
            ImageTarget.LessonImage => "lessons",
            ImageTarget.PostImage => "posts",
            ImageTarget.ReportImage => "reports",
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(folder))
            return url;

        var marker = $"/{folder}/";
        var idx = url.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx >= 0)
            return url[(idx + 1)..];

        return url;
    }

    private void ValidateFiles(List<IFormFile> files)
    {
        long totalSize = 0;

        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            if (file == null)
                continue;

            if (file.Length <= 0)
                throw new ArgumentException($"File ở vị trí {i + 1} không hợp lệ.");

            if (file.Length > MaxFileSizeBytes)
                throw new ArgumentException($"Dung lượng file '{file.FileName}' vượt quá 3MB.");

            totalSize += file.Length;
            if (totalSize > MaxTotalSizeBytes)
                throw new ArgumentException("Tổng dung lượng file vượt quá 10MB.");

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
                throw new ArgumentException($"Định dạng file '{file.FileName}' không được hỗ trợ.");

            if (!string.IsNullOrWhiteSpace(file.ContentType) && !AllowedContentTypes.Contains(file.ContentType))
                throw new ArgumentException($"Content-Type '{file.ContentType}' không hợp lệ.");
        }
    }
}
