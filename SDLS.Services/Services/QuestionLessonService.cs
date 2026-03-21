using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionLesson;
using SDLS.Model.Enumerations;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class QuestionLessonService : IQuestionLessonService
    {
        private readonly IQuestionLessonRepository _repository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

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

        public QuestionLessonService(
            IQuestionLessonRepository repository,
            IStorageService storageService,
            IMapper mapper)
        {
            _repository = repository;
            _storageService = storageService;
            _mapper = mapper;
        }

        public async Task<PagedResult<QuestionLessonDTO>> GetAllAsync(
            Guid? id = null,
            Guid? questionChapterId = null,
            string? name = null,
            string? description = null,
            string? content = null,
            int? status = 1,
            int page = 1,
            int pageSize = 20)
        {
            var filtered = await _repository.GetAllAsync(
                id,
                questionChapterId,
                name,
                description,
                content,
                status);

            var total = filtered.Count();

            var pageEntities = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var lessonIds = pageEntities.Select(x => x.Id).ToList();
            var lessonImages = await _repository.GetLessonImagesByLessonIdsAsync(lessonIds);
            var imageLookup = lessonImages.GroupBy(x => x.QuestionLessonId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var dtos = _mapper.Map<List<QuestionLessonDTO>>(pageEntities);

            foreach (var dto in dtos)
            {
                if (imageLookup.TryGetValue(dto.Id, out var images))
                {
                    dto.LessonImages = _mapper.Map<List<QuestionLessonImageDTO>>(images);
                }
            }

            return new PagedResult<QuestionLessonDTO>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<QuestionLessonDTO> GetByIdAsync(Guid id)
        {
            var lesson = await _repository.GetByIdAsync(id);
            if (lesson == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            var dto = _mapper.Map<QuestionLessonDTO>(lesson);
            var images = await _repository.GetLessonImagesByLessonIdsAsync(new List<Guid> { id });
            dto.LessonImages = _mapper.Map<List<QuestionLessonImageDTO>>(images);

            return dto;
        }

        public async Task<bool> CreateAsync(QuestionLessonCreateDTO dto)
        {
            ValidateImageUpload(dto.LessonImageFiles, dto.LessonImageNames);

            if (dto.QuestionChapterId == Guid.Empty)
                throw new ArgumentException("QuestionChapterId không hợp lệ.");

            var now = DateTime.UtcNow.ToLocalTime();

            var lesson = new QuestionLesson
            {
                Id = Guid.NewGuid(),
                QuestionChapterId = dto.QuestionChapterId,
                Name = dto.Name,
                Description = dto.Description,
                Content = dto.Content,
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _repository.AddAsync(lesson);

            if (dto.LessonImageFiles != null && dto.LessonImageFiles.Any())
            {
                var images = new List<LessonImage>();

                for (int i = 0; i < dto.LessonImageFiles.Count; i++)
                {
                    var file = dto.LessonImageFiles[i];
                    if (file == null || file.Length == 0)
                        continue;

                    var url = await _storageService.UploadImageAsync(file, ImageTarget.LessonImage, lesson.Id);

                    var inputName = dto.LessonImageNames != null && dto.LessonImageNames.Count > i
                        ? dto.LessonImageNames[i]
                        : null;

                    var finalName = ResolveImageName(file.FileName, inputName, i);

                    images.Add(new LessonImage
                    {
                        Id = Guid.NewGuid(),
                        QuestionLessonId = lesson.Id,
                        Name = finalName,
                        Url = url,
                        CreateAt = now,
                        UpdateAt = now,
                        Status = 1
                    });
                }

                if (images.Any())
                {
                    _repository.AddLessonImages(images);
                    await _repository.UpdateAsync(lesson);
                }
            }

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, QuestionLessonUpdateDTO dto)
        {
            ValidateImageUpload(dto.LessonImageFiles, dto.LessonImageNames);

            if (dto.QuestionChapterId == Guid.Empty)
                throw new ArgumentException("QuestionChapterId không hợp lệ.");

            var lesson = await _repository.GetByIdForUpdateAsync(id);
            if (lesson == null)
                throw new KeyNotFoundException("Không tìm thấy QuestionLesson");

            var now = DateTime.UtcNow.ToLocalTime();

            lesson.QuestionChapterId = dto.QuestionChapterId;
            lesson.Name = dto.Name;
            lesson.Description = dto.Description;
            lesson.Content = dto.Content;
            lesson.Status = dto.Status ?? lesson.Status ?? 1;
            lesson.UpdateAt = now;

            // Replace old images
            var oldImages = await _repository.GetLessonImagesByLessonIdForUpdateAsync(id);

            foreach (var oldImage in oldImages)
            {
                if (!string.IsNullOrWhiteSpace(oldImage.Url))
                {
                    await _storageService.DeleteImageAsync(oldImage.Url, ImageTarget.LessonImage);
                }
            }

            _repository.RemoveLessonImages(oldImages);

            if (dto.LessonImageFiles != null && dto.LessonImageFiles.Any())
            {
                var newImages = new List<LessonImage>();

                for (int i = 0; i < dto.LessonImageFiles.Count; i++)
                {
                    var file = dto.LessonImageFiles[i];
                    if (file == null || file.Length == 0)
                        continue;

                    var url = await _storageService.UploadImageAsync(file, ImageTarget.LessonImage, id);

                    var inputName = dto.LessonImageNames != null && dto.LessonImageNames.Count > i
                        ? dto.LessonImageNames[i]
                        : null;

                    var finalName = ResolveImageName(file.FileName, inputName, i);

                    newImages.Add(new LessonImage
                    {
                        Id = Guid.NewGuid(),
                        QuestionLessonId = id,
                        Name = finalName,
                        Url = url,
                        CreateAt = now,
                        UpdateAt = now,
                        Status = 1
                    });
                }

                if (newImages.Any())
                {
                    _repository.AddLessonImages(newImages);
                }
            }

            await _repository.UpdateAsync(lesson);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var lesson = await _repository.GetByIdForUpdateAsync(id);
            if (lesson == null)
                throw new KeyNotFoundException($"Không tìm thấy QuestionLesson với Id {id}");

            var now = DateTime.UtcNow.ToLocalTime();

            lesson.Status = 0;
            lesson.UpdateAt = now;

            await _repository.SoftDeleteLessonImagesAsync(id, now);
            await _repository.UpdateAsync(lesson);

            return true;
        }

        private static string ResolveImageName(string? fileName, string? inputName, int index)
        {
            if (!string.IsNullOrWhiteSpace(inputName))
                return inputName.Trim();

            if (!string.IsNullOrWhiteSpace(fileName))
                return fileName.Trim();

            return $"lesson-image-{DateTime.UtcNow:yyyyMMddHHmmss}-{index + 1}";
        }

        private static void ValidateImageUpload(
            List<IFormFile>? files,
            List<string>? imageNames)
        {
            if (files == null || files.Count == 0)
                return;

            long totalSize = 0;

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file == null)
                    continue;

                if (file.Length <= 0)
                    throw new ArgumentException($"Ảnh ở vị trí {i + 1} không hợp lệ.");

                if (file.Length > MaxFileSizeBytes)
                    throw new ArgumentException($"Dung lượng ảnh '{file.FileName}' vượt quá 3MB.");

                totalSize += file.Length;
                if (totalSize > MaxTotalSizeBytes)
                    throw new ArgumentException("Tổng dung lượng ảnh vượt quá 10MB.");

                var ext = Path.GetExtension(file.FileName);
                if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
                    throw new ArgumentException($"Định dạng file '{file.FileName}' không được hỗ trợ.");

                if (!string.IsNullOrWhiteSpace(file.ContentType) && !AllowedContentTypes.Contains(file.ContentType))
                    throw new ArgumentException($"Content-Type '{file.ContentType}' của file '{file.FileName}' không hợp lệ.");

                if (imageNames != null && imageNames.Count > i && !string.IsNullOrWhiteSpace(imageNames[i]) && imageNames[i]!.Length > 255)
                    throw new ArgumentException("Vượt quá độ dài tối đa 255 ký tự.");
            }
        }
    }
}