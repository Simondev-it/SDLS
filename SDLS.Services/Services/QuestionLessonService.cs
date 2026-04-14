using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.QuestionLesson;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;
using SDLS.Services.Utilities;

namespace SDLS.Services.Services
{
    public class QuestionLessonService : IQuestionLessonService
    {
        private readonly IQuestionLessonRepository _repository;
        private readonly IQuestionChapterRepository _questionChapterRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILessonImageService _lessonImageService;

        public QuestionLessonService(
            IQuestionLessonRepository repository,
            IQuestionChapterRepository questionChapterRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ILessonImageService lessonImageService)
        {
            _repository = repository;
            _questionChapterRepository = questionChapterRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _lessonImageService = lessonImageService;
        }

        public async Task<PagedResult<QuestionLessonDTO>> GetAllAsync(
            Guid? id = null,
            Guid? questionChapterId = null,
            string? name = null,
            string? description = null,
            string? content = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var filtered = await _repository.GetAllAsync(
                id, questionChapterId, name, description, content, status, role);

            var total = filtered.Count();

            var pageEntities = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var lessonIds = pageEntities.Select(x => x.Id).ToList();
            var lessonImages = await _repository.GetLessonImagesByLessonIdsAsync(lessonIds, role);
            var imageLookup = lessonImages.GroupBy(x => x.QuestionLessonId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var dtos = _mapper.Map<List<QuestionLessonDTO>>(pageEntities);

            foreach (var dto in dtos)
            {
                if (imageLookup.TryGetValue(dto.Id, out var images))
                    dto.LessonImages = _mapper.Map<List<QuestionLessonImageDTO>>(images);
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
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var lesson = await _repository.GetByIdAsync(id, role);
            if (lesson == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var dto = _mapper.Map<QuestionLessonDTO>(lesson);
            var images = await _repository.GetLessonImagesByLessonIdsAsync(new List<Guid> { id }, role);
            dto.LessonImages = _mapper.Map<List<QuestionLessonImageDTO>>(images);

            return dto;
        }

        public async Task<QuestionLessonDTO> CreateAsync(QuestionLessonCreateDTO dto)
        {
            if (dto.QuestionChapterId == Guid.Empty)
                throw ApiException.BadRequest("QuestionChapterId không hợp lệ.");

            var chapter = await _questionChapterRepository.GetByIdAsync(dto.QuestionChapterId);
            if (chapter == null)
                throw ApiException.BadRequest("Không tìm thấy QuestionChapter với Id " + dto.QuestionChapterId);

            var now = DateTimeHelper.GetVietnamNow();

            var lesson = new QuestionLesson
            {
                Id = Guid.NewGuid(),
                QuestionChapterId = dto.QuestionChapterId,
                Index = dto.Index,
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

            return _mapper.Map<QuestionLessonDTO>(lesson);
        }

        public async Task<QuestionLessonDTO> UpdateAsync(Guid id, QuestionLessonUpdateDTO dto)
        {
            var lesson = await _repository.GetByIdForUpdateAsync(id);
            if (lesson == null)
                throw ApiException.NotFound("Không tìm thấy QuestionLesson");

            var now = DateTimeHelper.GetVietnamNow();
            var changed = false;

            if (dto.QuestionChapterId.HasValue)
            {
                if (dto.QuestionChapterId.Value == Guid.Empty)
                    throw ApiException.BadRequest("QuestionChapterId không hợp lệ.");

                var chapter = await _questionChapterRepository.GetByIdAsync(dto.QuestionChapterId.Value);
                if (chapter == null)
                    throw ApiException.BadRequest("Không tìm thấy QuestionChapter với Id " + dto.QuestionChapterId);

                if (lesson.QuestionChapterId != dto.QuestionChapterId.Value)
                {
                    lesson.QuestionChapterId = dto.QuestionChapterId.Value;
                    changed = true;
                }
            }

            if (dto.Index.HasValue && lesson.Index != dto.Index.Value)
            {
                lesson.Index = dto.Index.Value;
                changed = true;
            }

            if (dto.Name != null)
            {
                var newName = dto.Name.Trim();
                if (string.IsNullOrWhiteSpace(newName))
                    throw ApiException.BadRequest("Name không được để trống.");

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
                return _mapper.Map<QuestionLessonDTO>(lesson);

            lesson.UpdateAt = now;

            await _repository.UpdateAsync(lesson);
            return _mapper.Map<QuestionLessonDTO>(lesson);
        }

        public async Task<QuestionLessonDTO> DeleteAsync(Guid id)
        {
            return await DeleteSoftAsync(id);
        }

        public async Task<QuestionLessonDTO> DeleteSoftAsync(Guid id)
        {
            var lesson = await _repository.GetByIdForUpdateAsync(id);
            if (lesson == null)
                throw ApiException.NotFound($"Không tìm thấy QuestionLesson với Id {id}");

            var now = DateTimeHelper.GetVietnamNow();

            var currentStatus = lesson.Status ?? 1;
            var nextStatus = currentStatus == 0 ? 1 : 0;

            lesson.Status = nextStatus;
            lesson.UpdateAt = now;

            if (nextStatus == 0)
                await _repository.SoftDeleteLessonImagesAsync(id, now);
            else
                await _repository.RestoreLessonImagesAsync(id, now);

            await _repository.UpdateAsync(lesson);

            return _mapper.Map<QuestionLessonDTO>(lesson);
        }

        public async Task<QuestionLessonDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var lesson = await _repository.GetByIdAsync(id, role);
            if (lesson == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<QuestionLessonDTO>(lesson);
            await _repository.DeleteHardAsync(id);
            return result;
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
                foreach (var img in imagesToRemove)
                {
                    await _lessonImageService.DeleteAsync(img.Id);
                }
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