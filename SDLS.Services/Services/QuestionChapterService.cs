using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionChapter;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using System.Globalization;
using System.Text;

namespace SDLS.Services.Services
{
    public class QuestionChapterService : IQuestionChapterService
    {
        private readonly IQuestionChapterRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public QuestionChapterService(
            IQuestionChapterRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<PagedResult<QuestionChapterDTO>> GetAllAsync(
            Guid? id = null,
            Guid? drivingLicenseId = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var filtered = await _repository.GetAllAsync(
                id, drivingLicenseId, name, description, status, role);

            var total = filtered.Count();

            var items = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<QuestionChapterDTO>
            {
                Items = _mapper.Map<List<QuestionChapterDTO>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<QuestionChapterDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<QuestionChapterDTO>(entity);
        }

        public async Task<bool> CreateAsync(QuestionChapterCreateDTO dto)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var entity = _mapper.Map<QuestionChapter>(dto);
            entity.Id = Guid.NewGuid();
            entity.Index = dto.Index;
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Status = 1;

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, QuestionChapterUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy QuestionChapter");

            existing.DrivingLicenseId = dto.DrivingLicenseId;
            existing.Index = dto.Index;
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

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