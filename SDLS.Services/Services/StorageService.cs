using Microsoft.AspNetCore.Http;
using SDLS.Model.Enumerations;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;
using Supabase.Storage.Exceptions;

namespace SDLS.Services.Services
{
    public static class StorageBucket
    {
        public const string Public = "sdls-public";
        public const string Content = "sdls-content";
        public const string Private = "sdls-private";
    }

    public class StorageService : IStorageService
    {
        private readonly Supabase.Client _supabaseClient;

        private static readonly Dictionary<ImageTarget, (string Bucket, string Folder)> _config = new()
        {
            { ImageTarget.UserAvatar,        (StorageBucket.Public,  "avatars")       },
            { ImageTarget.TrafficSign,       (StorageBucket.Public,  "traffic-signs") },
            { ImageTarget.NotificationImage, (StorageBucket.Public,  "notifications") },
            { ImageTarget.QuestionImage,     (StorageBucket.Content, "questions")     },
            { ImageTarget.LessonImage,       (StorageBucket.Content, "lessons")       },
            { ImageTarget.PostImage,         (StorageBucket.Content, "forum/posts")   },
            { ImageTarget.ReportImage,       (StorageBucket.Private, "reports")       },
        };

        // 1 entity = 1 ảnh → dùng entityId làm fileName
        private static readonly HashSet<ImageTarget> _singleImageTargets = new()
        {
            ImageTarget.UserAvatar,
            ImageTarget.TrafficSign,
            ImageTarget.NotificationImage,
            ImageTarget.QuestionImage,
            ImageTarget.ReportImage,
        };

        public StorageService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<string> UploadImageAsync(IFormFile file, ImageTarget target, Guid entityId)
        {
            if (file is null) throw ApiException.BadRequest("File is required.");
            if (file.Length <= 0) throw ApiException.BadRequest("File is empty.");
            if (!_config.TryGetValue(target, out var cfg))
                throw ApiException.BadRequest($"No storage config for target '{target}'.");

            var (bucket, folder) = cfg;

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext))
                ext = MimeTypeToExtension(file.ContentType);
            
            string filePath;
            if (_singleImageTargets.Contains(target))
            {
                filePath = $"{folder}/{entityId}{ext}"; // 1 ảnh  → avatars/{userId}.jpg, ghi đè nếu đã tồn tại
            }
            else // nhiều  → lessons/{lessonId}/{newGuid}.jpg để tránh ghi đè và có thể lưu nhiều ảnh cho cùng 1 entity
            {
                filePath = $"{folder}/{entityId}/{Guid.NewGuid()}{ext}";
            }

            byte[] bytes;
            await using (var stream = file.OpenReadStream())
            {
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            // supabase-csharp (0.16.x): content type is passed as a separate argument.
            // FileOptions does not contain ContentType; it supports Upsert/CacheControl.
            var options = new Supabase.Storage.FileOptions
            {
                Upsert = _singleImageTargets.Contains(target),
                CacheControl = "3600",
                ContentType = file.ContentType
            };

            var storage = _supabaseClient.Storage.From(bucket);
            try
            {
                await storage.Upload(bytes, filePath, options);
            }
            catch (SupabaseStorageException ex)
            {
                throw ApiException.Internal(
                    $"Failed to upload image to Supabase storage bucket '{bucket}' at path '{filePath}'. " +
                    "This is usually caused by Storage RLS policy restrictions. For backend uploads, use Supabase service role key in configuration ('Supabase:ServiceRoleKey') or adjust bucket policies.");
            }

            return storage.GetPublicUrl(filePath);
        }

        public async Task<bool> DeleteImageAsync(string fileUrl, ImageTarget target)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return true;

            if (!_config.TryGetValue(target, out var cfg))
                throw ApiException.BadRequest($"No storage config for target '{target}'.");

            var (bucket, _) = cfg;
            var path = ExtractPathFromUrl(fileUrl, bucket);

            try
            {
                await _supabaseClient.Storage
                    .From(bucket)
                    .Remove(new List<string> { path });
            }
            catch (SupabaseStorageException ex)
            {
                throw ApiException.Internal(
                    $"Failed to delete image from Supabase storage bucket '{bucket}' at path '{path}'. " +
                    "Check Storage RLS policies or service role key configuration.");
            }

            return true;
        }

        private static string ExtractPathFromUrl(string url, string bucket)
        {
            var publicMarker = $"/object/public/{bucket}/";

            var idx = url.IndexOf(publicMarker, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
                return url[(idx + publicMarker.Length)..];

            return url;
        }

        private static string? MimeTypeToExtension(string? contentType)
        {
            return contentType?.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/webp" => ".webp",
                _ => null
            };
        }
    }
}

