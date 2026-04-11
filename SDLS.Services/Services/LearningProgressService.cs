using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.LearningProgress;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class LearningProgressService : ILearningProgressService
    {
        private readonly ILearningProgressRepository _repository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public LearningProgressService(
            ILearningProgressRepository repository,
            IQuestionRepository questionRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _questionRepository = questionRepository;
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
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<LearningProgressDTO>(entity);
        }

        public async Task<LearningProgressDTO> CreateAsync(LearningProgressCreateDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.QuestionId == Guid.Empty)
                throw ApiException.BadRequest("QuestionId không được rỗng");

            var questionExists = await _questionRepository.GetByIdAsync(dto.QuestionId);
            if (questionExists == null)
                throw ApiException.BadRequest("QuestionId không tồn tại");

            var existing = await _repository.GetByUserAndQuestionAsync(currentUserId, dto.QuestionId);
            if (existing != null && existing.Any())
                throw ApiException.Conflict("LearningProgress cho UserId và QuestionId này đã tồn tại.");

            var entity = _mapper.Map<LearningProgress>(dto);
            entity.Id = Guid.NewGuid();
            entity.UserId = currentUserId;
            entity.CreateAt = DateTimeHelper.GetVietnamNow();
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            entity.Status = 1;

            await _repository.AddAsync(entity);
            return _mapper.Map<LearningProgressDTO>(entity);
        }

        public async Task<LearningProgressDTO> UpdateAsync(Guid id, LearningProgressUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id, null);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy LearningProgress");

            var questionExists = await _questionRepository.GetByIdAsync(dto.QuestionId);
            if (questionExists == null)
                throw ApiException.BadRequest("QuestionId không tồn tại");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            var isChangingKeys = existing.UserId != currentUserId || existing.QuestionId != dto.QuestionId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndQuestionAsync(currentUserId, dto.QuestionId);
                if (conflict != null && conflict.Any(lp => lp.Id != id))
                    throw ApiException.Conflict("Cặp UserId và QuestionId mới đã tồn tại ở record khác.");
            }

            existing.QuestionId = dto.QuestionId;
            existing.UserId = currentUserId;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateAsync(existing);
            return _mapper.Map<LearningProgressDTO>(existing);
        }

        public async Task<LearningProgressDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var existing = await _repository.GetByIdAsync(id, role);
            if (existing == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<LearningProgressDTO>(existing);
        }

        public async Task<LearningProgressDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var existing = await _repository.GetByIdAsync(id, role);
            if (existing == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteHardAsync(id);
            return _mapper.Map<LearningProgressDTO>(existing);
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
