using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionLesson;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class QuestionLessonService : IQuestionLessonService
    {
        private readonly IQuestionLessonRepository _repository;
        private readonly IMapper _mapper;

        public QuestionLessonService(IQuestionLessonRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<QuestionLessonDTO>> GetAllAsync(
            Guid? id = null,
            Guid? questionChapterId = null,
            string? name = null,
            string? description = null,
            int? status = 1,
            int page = 1,
            int pageSize = 20)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable();

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (questionChapterId.HasValue)
                filtered = filtered.Where(x => x.QuestionChapterId == questionChapterId.Value);

            if (!string.IsNullOrWhiteSpace(name))
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.Name)
                    && x.Name.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(description))
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.Description)
                    && x.Description.Contains(description.Trim(), StringComparison.OrdinalIgnoreCase));

            if (status.HasValue)
                filtered = filtered.Where(x => x.Status == status.Value);

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
            var now = DateTime.UtcNow.ToLocalTime();

            var lesson = _mapper.Map<QuestionLesson>(dto);
            lesson.Id = Guid.NewGuid();
            lesson.CreateAt = now;
            lesson.UpdateAt = now;
            lesson.Status = 1;

            await _repository.AddAsync(lesson);

            if (dto.LessonImages != null && dto.LessonImages.Any())
            {
                var images = dto.LessonImages.Select(x => new LessonImage
                {
                    Id = Guid.NewGuid(),
                    QuestionLessonId = lesson.Id,
                    Name = x.Name,
                    Url = x.Url,
                    CreateAt = now,
                    UpdateAt = now,
                    Status = 1
                }).ToList();

                _repository.AddLessonImages(images);
                await _repository.UpdateAsync(lesson);
            }

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, QuestionLessonUpdateDTO dto)
        {
            var lesson = await _repository.GetByIdForUpdateAsync(id);
            if (lesson == null)
                throw new KeyNotFoundException("Không tìm thấy QuestionLesson");

            var now = DateTime.UtcNow.ToLocalTime();

            lesson.QuestionChapterId = dto.QuestionChapterId;
            lesson.Name = dto.Name;
            lesson.Description = dto.Description;
            lesson.Status = dto.Status ?? lesson.Status ?? 1;
            lesson.UpdateAt = now;

            var oldImages = await _repository.GetLessonImagesByLessonIdForUpdateAsync(id);
            _repository.RemoveLessonImages(oldImages);

            if (dto.LessonImages != null && dto.LessonImages.Any())
            {
                var newImages = dto.LessonImages.Select(x => new LessonImage
                {
                    Id = Guid.NewGuid(),
                    QuestionLessonId = id,
                    Name = x.Name,
                    Url = x.Url,
                    CreateAt = now,
                    UpdateAt = now,
                    Status = x.Status ?? 1
                }).ToList();

                _repository.AddLessonImages(newImages);
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
    }
}