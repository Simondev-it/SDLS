using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.LessonProgress;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class LessonProgressService : ILessonProgressService
    {
        private readonly ILessonProgressRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public LessonProgressService(
            ILessonProgressRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<LessonProgressDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionLessonId = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetAllAsync(id, userId, questionLessonId, status, role);
            return _mapper.Map<List<LessonProgressDTO>>(entities);
        }

        public async Task<PagedResult<LessonProgressDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionLessonId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, questionLessonId, status);
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
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            return entity != null ? _mapper.Map<LessonProgressDTO>(entity) : null;
        }

        public async Task<List<LessonProgressDTO>> GetByUserIdAsync(Guid userId, int? status = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId không được rỗng");

            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetByUserAndQuestionLessonAsync(userId, null, status, role);
            return _mapper.Map<List<LessonProgressDTO>>(entities);
        }

        public async Task<bool> CreateAsync(LessonProgressCreateDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.QuestionLessonId == Guid.Empty)
                throw new ArgumentException("QuestionLessonId không được rỗng");

            var existing = await _repository.GetByUserAndQuestionLessonAsync(currentUserId, dto.QuestionLessonId);
            if (existing != null && existing.Any())
                throw new InvalidOperationException("LessonProgress cho UserId và QuestionLessonId này đã tồn tại.");

            var entity = _mapper.Map<LessonProgress>(dto);
            entity.Id = Guid.NewGuid();
            entity.UserId = currentUserId;
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

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            var isChangingKeys = existing.UserId != currentUserId || existing.QuestionLessonId != dto.QuestionLessonId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndQuestionLessonAsync(currentUserId, dto.QuestionLessonId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw new InvalidOperationException("Cặp UserId và QuestionLessonId mới đã tồn tại ở record khác.");
            }

            existing.UserId = currentUserId;
            existing.QuestionLessonId = dto.QuestionLessonId;
            existing.Score = dto.Score ?? existing.Score;
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