using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Role;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<RoleDTO>> GetListAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var all = await _repository.GetAllAsync(id, name, description, status, role);
            var ordered = all.OrderBy(x => x.Name).ThenBy(x => x.Id).ToList();
            return _mapper.Map<List<RoleDTO>>(ordered);
        }

        public async Task<PagedResult<RoleDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetListAsync(id, name, description, status);
            var total = items.Count;

            return new PagedResult<RoleDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<RoleDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<RoleDTO>(entity);
        }

        public async Task<RoleDTO> CreateAsync(RoleCreateDTO dto)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var entity = new Role
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _repository.AddAsync(entity);
            return _mapper.Map<RoleDTO>(entity);
        }

        public async Task<RoleDTO> UpdateAsync(Guid id, RoleUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy Role");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<RoleDTO>(existing);
        }

        public async Task<RoleDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var existing = await _repository.GetByIdAsync(id, role);
            if (existing == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            return _mapper.Map<RoleDTO>(existing);
        }

        public async Task<RoleDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var existing = await _repository.GetByIdAsync(id, role);
            if (existing == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<RoleDTO>(existing);
            await _repository.DeleteHardAsync(id);
            return result;
        }
    }
}