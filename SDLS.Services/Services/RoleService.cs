using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Role;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<RoleDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable().Where(x => x.Status == 1);

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var keyword = name.Trim();
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.Name)
                    && x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var keyword = description.Trim();
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.Description)
                    && x.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            var ordered = filtered.OrderBy(x => x.Name).ThenBy(x => x.Id).ToList();
            var total = ordered.Count;

            var items = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<RoleDTO>
            {
                Items = _mapper.Map<List<RoleDTO>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<RoleDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<RoleDTO>(entity);
        }

        public async Task<bool> CreateAsync(RoleCreateDTO dto)
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
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, RoleUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy Role");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
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