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

            await SyncLessonImagesFromContentAsync(id, dto.Content, now);

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

        private async Task SyncLessonImagesFromContentAsync(Guid lessonId, string? content, DateTime now)
        {
            var newUrls = HtmlContentParser.ExtractImageUrls(content);
            var activeImages = await _repository.GetLessonImagesByLessonIdForUpdateAsync(lessonId);

            var activeUrlSet = activeImages
                .Where(x => !string.IsNullOrWhiteSpace(x.Url))
                .Select(x => x.Url!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var image in activeImages)
            {
                if (string.IsNullOrWhiteSpace(image.Url))
                    continue;

                if (!newUrls.Contains(image.Url, StringComparer.OrdinalIgnoreCase))
                {
                    image.Status = 0;
                    image.UpdateAt = now;
                }
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