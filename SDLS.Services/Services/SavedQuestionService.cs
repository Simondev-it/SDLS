using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SavedQuestion;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SavedQuestionService : ISavedQuestionService
    {
        private readonly ISavedQuestionRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public SavedQuestionService(
            ISavedQuestionRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<SavedQuestionDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetAllAsync(id, userId, questionId, status, role);
            return _mapper.Map<List<SavedQuestionDTO>>(entities);
        }

        public async Task<PagedResult<SavedQuestionDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, questionId, status);
            var total = items.Count;

            return new PagedResult<SavedQuestionDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<SavedQuestionDTO?> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            return entity != null ? _mapper.Map<SavedQuestionDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(SavedQuestionCreateDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.QuestionId == Guid.Empty)
                throw new ArgumentException("QuestionId không được rỗng");

            var existing = await _repository.GetByUserAndQuestionAsync(currentUserId, dto.QuestionId);
            if (existing != null && existing.Any())
                throw new InvalidOperationException("SavedQuestion cho UserId và QuestionId này đã tồn tại.");

            var entity = new SavedQuestion
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                QuestionId = dto.QuestionId,
                CreateAt = DateTime.UtcNow.ToLocalTime(),
                UpdateAt = DateTime.UtcNow.ToLocalTime(),
                Status = 1
            };

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, SavedQuestionUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            var isChangingKeys = existing.UserId != currentUserId || existing.QuestionId != dto.QuestionId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndQuestionAsync(currentUserId, dto.QuestionId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw new InvalidOperationException("Cặp UserId và QuestionId mới đã tồn tại ở record khác.");
            }

            existing.UserId = currentUserId;
            existing.QuestionId = dto.QuestionId;
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