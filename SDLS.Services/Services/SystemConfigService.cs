using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SystemConfig;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SystemConfigService : ISystemConfigService
    {
        private readonly ISystemConfigRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public SystemConfigService(
            ISystemConfigRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<SystemConfigDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            int? value = null,
            string? description = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetAllAsync(id, name, value, description, status, role);
            return _mapper.Map<List<SystemConfigDTO>>(entities);
        }

        public async Task<PagedResult<SystemConfigDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            int? value = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, name, value, description, status);
            var total = items.Count;

            return new PagedResult<SystemConfigDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<SystemConfigDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<SystemConfigDTO>(entity);
        }

        public async Task<SystemConfigDTO> CreateAsync(SystemConfigCreateDTO dto)
        {
            var now = DateTimeHelper.GetVietnamNow();

            var entity = new SystemConfig
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Value = dto.Value,
                Description = dto.Description,
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _repository.AddAsync(entity);
            return _mapper.Map<SystemConfigDTO>(entity);
        }

        public async Task<SystemConfigDTO> UpdateAsync(Guid id, SystemConfigUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm th?y SystemConfig");

            existing.Name = dto.Name;
            existing.Value = dto.Value;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<SystemConfigDTO>(existing);
        }

        public async Task<SystemConfigDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            entity.Status = 0;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<SystemConfigDTO>(entity);
        }

        public async Task<SystemConfigDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<SystemConfigDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }
    }
}
