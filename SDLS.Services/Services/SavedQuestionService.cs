using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SavedQuestion;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SavedQuestionService : ISavedQuestionService
    {
        private readonly ISavedQuestionRepository _repository;
        private readonly IMapper _mapper;

        public SavedQuestionService(ISavedQuestionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SavedQuestionDTO>> GetAllAsync(Guid? id = null, Guid? userId = null, Guid? questionId = null)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable();

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                filtered = filtered.Where(x => x.UserId == userId.Value);

            if (questionId.HasValue)
                filtered = filtered.Where(x => x.QuestionId == questionId.Value);

            return _mapper.Map<List<SavedQuestionDTO>>(filtered.ToList());
        }

        public async Task<PagedResult<SavedQuestionDTO>> GetPagedAsync(Guid? id = null, Guid? userId = null, Guid? questionId = null, int page = 1, int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, questionId);
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
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<SavedQuestionDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(SavedQuestionCreateDTO dto)
        {
            if (dto.UserId == Guid.Empty || dto.QuestionId == Guid.Empty)
                throw new ArgumentException("UserId và QuestionId không được rỗng");

            var existing = await _repository.GetByUserAndQuestionAsync(dto.UserId, dto.QuestionId);
            if (existing != null && existing.Any())
                throw new InvalidOperationException("SavedQuestion cho UserId và QuestionId này đã tồn tại.");

            var entity = new SavedQuestion
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
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

            var isChangingKeys = existing.UserId != dto.UserId || existing.QuestionId != dto.QuestionId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndQuestionAsync(dto.UserId, dto.QuestionId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw new InvalidOperationException("Cặp UserId và QuestionId mới đã tồn tại ở record khác.");
            }

            existing.UserId = dto.UserId;
            existing.QuestionId = dto.QuestionId;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}