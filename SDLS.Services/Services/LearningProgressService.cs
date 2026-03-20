using AutoMapper;
using SDLS.Model.DTOs.LearningProgress;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.Services
{
    public class LearningProgressService : ILearningProgressService
    {
        private readonly ILearningProgressRepository _repository;
        private readonly IMapper _mapper;

        public LearningProgressService(ILearningProgressRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LearningProgressDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<List<LearningProgressDTO>>(entities);
        }

        public async Task<LearningProgressDTO?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<LearningProgressDTO>(entity) : null;
        }

        public async Task<LearningProgressDTO> CreateAsync(LearningProgressCreateDTO dto)
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

            return _mapper.Map<LearningProgressDTO>(entity);
        }

        public async Task<LearningProgressDTO?> UpdateAsync(Guid id, LearningProgressUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            // Kiểm tra nếu thay đổi UserId hoặc QuestionId → tránh trùng lặp
            bool isChangingKeys = existing.UserId != dto.UserId || existing.QuestionId != dto.QuestionId;

            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndQuestionAsync(dto.UserId, dto.QuestionId);
                if (conflict != null && conflict.Any(lp => lp.Id != id)) // loại trừ chính bản thân
                    throw new InvalidOperationException("Cặp UserId và QuestionId mới đã tồn tại ở record khác.");
            }

            // Update fields
            existing.QuestionId = dto.QuestionId;
            existing.UserId = dto.UserId;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateAsync(existing);

            return _mapper.Map<LearningProgressDTO>(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<List<LearningProgressDTO>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId)
        {
            var entities = await _repository.GetByUserAndQuestionAsync(userId, questionId);
            return _mapper.Map<List<LearningProgressDTO>>(entities);
        }
    }
}
