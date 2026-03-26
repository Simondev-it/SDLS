using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.LearningProgress;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class LearningProgressService : ILearningProgressService
    {
        private readonly ILearningProgressRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public LearningProgressService(
            ILearningProgressRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LearningProgressDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetAllAsync(id, userId, questionId, status, role);
            return _mapper.Map<List<LearningProgressDTO>>(entities);
        }

        public async Task<PagedResult<LearningProgressDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var items = (await GetAllAsync(id, userId, questionId, status)).ToList();
            var total = items.Count;

            return new PagedResult<LearningProgressDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<LearningProgressDTO?> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            return entity != null ? _mapper.Map<LearningProgressDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(LearningProgressCreateDTO dto)
        {
            if (dto.QuestionId == Guid.Empty || dto.UserId == Guid.Empty)
                throw new ArgumentException("QuestionId và UserId không được rỗng");

            var existing = await _repository.GetByUserAndQuestionAsync(dto.UserId, dto.QuestionId);
            if (existing != null && existing.Any())
                throw new InvalidOperationException("LearningProgress cho UserId và QuestionId này đã tồn tại.");

            var entity = _mapper.Map<LearningProgress>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreateAt = DateTime.UtcNow.ToLocalTime();
            entity.UpdateAt = DateTime.UtcNow.ToLocalTime();
            entity.Status = 1;

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, LearningProgressUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id, null);
            if (existing == null) return false;

            var isChangingKeys = existing.UserId != dto.UserId || existing.QuestionId != dto.QuestionId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndQuestionAsync(dto.UserId, dto.QuestionId);
                if (conflict != null && conflict.Any(lp => lp.Id != id))
                    throw new InvalidOperationException("Cặp UserId và QuestionId mới đã tồn tại ở record khác.");
            }

            existing.QuestionId = dto.QuestionId;
            existing.UserId = dto.UserId;
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

        public async Task<List<LearningProgressDTO>> GetByUserAndQuestionAsync(
            Guid? userId,
            Guid? questionId,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetByUserAndQuestionAsync(userId, questionId, status, role);
            return _mapper.Map<List<LearningProgressDTO>>(entities);
        }
    }
}
