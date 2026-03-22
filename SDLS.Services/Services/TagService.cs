using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Tag;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repository;
        private readonly IMapper _mapper;

        public TagService(ITagRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<TagDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            string? colorCode = null)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable()
                .Where(x => x.Status == 1);

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (!string.IsNullOrWhiteSpace(name))
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.Name)
                    && x.Name.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(description))
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.Description)
                    && x.Description.Contains(description.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(colorCode))
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.ColorCode)
                    && x.ColorCode.Contains(colorCode.Trim(), StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<List<TagDTO>>(filtered.ToList());
        }

        public async Task<PagedResult<TagDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            string? colorCode = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, name, description, colorCode);
            var total = items.Count;

            return new PagedResult<TagDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<TagDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<TagDTO>(entity);
        }

        public async Task<bool> CreateAsync(TagCreateDTO dto)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var entity = new Tag
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                ColorCode = dto.ColorCode,
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, TagUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy Tag");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.ColorCode = dto.ColorCode;
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
    }
}