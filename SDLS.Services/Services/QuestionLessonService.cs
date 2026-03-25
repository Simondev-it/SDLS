using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionLesson;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using SDLS.Services.Utilities;

namespace SDLS.Services.Services
{
    public class QuestionLessonService : IQuestionLessonService
    {
        private readonly IQuestionLessonRepository _repository;
        private readonly IMapper _mapper;

        public QuestionLessonService(
            IQuestionLessonRepository repository,
            IMapper mapper)
        {
            _repository = repository;
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

            await SyncLessonImagesFromContentAsync(lesson.Id, dto.Content, now);
            await _repository.UpdateAsync(lesson);

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, QuestionLessonUpdateDTO dto)
        {
            var lesson = await _repository.GetByIdForUpdateAsync(id);
            if (lesson == null)
                throw new KeyNotFoundException("Không tìm thấy QuestionLesson");

            var now = DateTime.UtcNow.ToLocalTime();
            var changed = false;

            if (dto.QuestionChapterId.HasValue)
            {
                if (dto.QuestionChapterId.Value == Guid.Empty)
                    throw new ArgumentException("QuestionChapterId không hợp lệ.");

                if (lesson.QuestionChapterId != dto.QuestionChapterId.Value)
                {
                    lesson.QuestionChapterId = dto.QuestionChapterId.Value;
                    changed = true;
                }
            }

            if (dto.Name != null)
            {
                var newName = dto.Name.Trim();
                if (string.IsNullOrWhiteSpace(newName))
                    throw new ArgumentException("Name không được để trống.");

                if (!string.Equals(lesson.Name, newName, StringComparison.Ordinal))
                {
                    lesson.Name = newName;
                    changed = true;
                }
            }

            if (dto.Description != null && !string.Equals(lesson.Description, dto.Description, StringComparison.Ordinal))
            {
                lesson.Description = dto.Description;
                changed = true;
            }

            if (dto.Content != null && !string.Equals(lesson.Content, dto.Content, StringComparison.Ordinal))
            {
                lesson.Content = dto.Content;
                await SyncLessonImagesFromContentAsync(id, dto.Content, now);
                changed = true;
            }

            if (dto.Status.HasValue)
            {
                var nextStatus = dto.Status.Value;
                if (lesson.Status != nextStatus)
                {
                    lesson.Status = nextStatus;
                    changed = true;
                }
            }

            if (!changed)
                return true;

            lesson.UpdateAt = now;

            await _repository.UpdateAsync(lesson);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await DeleteSoftAsync(id);
        }

        public async Task<bool> DeleteSoftAsync(Guid id)
        {
            var lesson = await _repository.GetByIdForUpdateAsync(id);
            if (lesson == null || lesson.Status != 1)
                throw new KeyNotFoundException($"Không tìm thấy QuestionLesson với Id {id}");

            var now = DateTime.UtcNow.ToLocalTime();

            lesson.Status = 0;
            lesson.UpdateAt = now;

            await _repository.SoftDeleteLessonImagesAsync(id, now);
            await _repository.UpdateAsync(lesson);

            return true;
        }

        public async Task<bool> DeleteHardAsync(Guid id)
        {
            await _repository.DeleteHardAsync(id);
            return true;
        }

        private async Task SyncLessonImagesFromContentAsync(Guid lessonId, string? content, DateTime now)
        {
            var newUrls = HtmlContentParser.ExtractImageUrls(content);
            var activeImages = await _repository.GetLessonImagesByLessonIdForUpdateAsync(lessonId);

            var activeUrlSet = activeImages
                .Where(x => !string.IsNullOrWhiteSpace(x.Url))
                .Select(x => x.Url!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var imagesToRemove = activeImages
                .Where(x => !string.IsNullOrWhiteSpace(x.Url)
                    && !newUrls.Contains(x.Url!, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (imagesToRemove.Any())
            {
                _repository.RemoveLessonImages(imagesToRemove);
            }

            var imagesToAdd = new List<LessonImage>();
            foreach (var url in newUrls)
            {
                if (activeUrlSet.Contains(url))
                    continue;

                imagesToAdd.Add(new LessonImage
                {
                    Id = Guid.NewGuid(),
                    QuestionLessonId = lessonId,
                    Name = HtmlContentParser.ResolveImageNameFromUrl(url),
                    Url = url,
                    CreateAt = now,
                    UpdateAt = now,
                    Status = 1
                });
            }

            if (imagesToAdd.Any())
            {
                _repository.AddLessonImages(imagesToAdd);
            }
        }

    }
}