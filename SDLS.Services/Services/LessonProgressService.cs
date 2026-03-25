    using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.LessonProgress;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class LessonProgressService : ILessonProgressService
    {
        private readonly ILessonProgressRepository _repository;
        private readonly IMapper _mapper;

        public LessonProgressService(ILessonProgressRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<LessonProgressDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionLessonId = null)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable();

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                filtered = filtered.Where(x => x.UserId == userId.Value);

            if (questionLessonId.HasValue)
                filtered = filtered.Where(x => x.QuestionLessonId == questionLessonId.Value);

            return _mapper.Map<List<LessonProgressDTO>>(filtered.ToList());
        }

        public async Task<PagedResult<LessonProgressDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionLessonId = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, questionLessonId);
            var total = items.Count;

            return new PagedResult<LessonProgressDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<LessonProgressDTO?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<LessonProgressDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(LessonProgressCreateDTO dto)
        {
            if (dto.UserId == Guid.Empty || dto.QuestionLessonId == Guid.Empty)
                throw new ArgumentException("UserId và QuestionLessonId không được rỗng");

            var existing = await _repository.GetByUserAndQuestionLessonAsync(dto.UserId, dto.QuestionLessonId);
            if (existing != null && existing.Any())
                throw new InvalidOperationException("LessonProgress cho UserId và QuestionLessonId này đã tồn tại.");

            var entity = _mapper.Map<LessonProgress>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreateAt = DateTime.UtcNow.ToLocalTime();
            entity.UpdateAt = DateTime.UtcNow.ToLocalTime();
            entity.Status = 1;

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, LessonProgressUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            var isChangingKeys = existing.UserId != dto.UserId || existing.QuestionLessonId != dto.QuestionLessonId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndQuestionLessonAsync(dto.UserId, dto.QuestionLessonId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw new InvalidOperationException("Cặp UserId và QuestionLessonId mới đã tồn tại ở record khác.");
            }

            existing.UserId = dto.UserId;
            existing.QuestionLessonId = dto.QuestionLessonId;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteSoftAsync(Guid id)
        {
            await _repository.DeleteSoftAsync(id);
            return true;
        }

        public async Task<bool> DeleteHardAsync(Guid id)
        {
            await _repository.DeleteHardAsync(id);
            return true;
        }
    }
}