using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.ReportCategory;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class ReportCategoryService : IReportCategoryService
    {
        private readonly IReportCategoryRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ReportCategoryService(
            IReportCategoryRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<ReportCategoryDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var all = await _repository.GetAllAsync(id, name, description, status, role);
            return _mapper.Map<List<ReportCategoryDTO>>(all);
        }

        public async Task<PagedResult<ReportCategoryDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, name, description, status);
            var total = items.Count;

            return new PagedResult<ReportCategoryDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<ReportCategoryDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<ReportCategoryDTO>(entity);
        }

        public async Task<ReportCategoryDTO> CreateAsync(ReportCategoryCreateDTO dto)
        {
            var now = DateTimeHelper.GetVietnamNow();

            var entity = new ReportCategory
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _repository.AddAsync(entity);
            return _mapper.Map<ReportCategoryDTO>(entity);
        }

        public async Task<ReportCategoryDTO> UpdateAsync(Guid id, ReportCategoryUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy ReportCategory");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<ReportCategoryDTO>(existing);
        }

        public async Task<ReportCategoryDTO> DeleteSoftAsync(Guid id)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            if (existing.Status != 0 && existing.Status != 1)
                throw ApiException.BadRequest("Chỉ hỗ trợ chuyển trạng thái giữa 0 và 1.");

            existing.Status = existing.Status == 1 ? 0 : 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _repository.UpdateAsync(existing);
            return _mapper.Map<ReportCategoryDTO>(existing);
        }

        public async Task<ReportCategoryDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var existing = await _repository.GetByIdAsync(id, role);
            if (existing == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<ReportCategoryDTO>(existing);
            await _repository.DeleteHardAsync(id);
            return result;
        }
    }
}