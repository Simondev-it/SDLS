using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.TrafficSign;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class TrafficSignService : ITrafficSignService
    {
        private readonly ITrafficSignRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public TrafficSignService(
            ITrafficSignRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<PagedResult<TrafficSignDTO>> GetAllAsync(
            Guid? id = null,
            Guid? signCategoryId = null,
            string? name = null,
            string? code = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var filtered = await _repository.GetAllAsync(id, signCategoryId, name, code, description, status, role);

            var ordered = filtered.OrderBy(x => x.Code).ThenBy(x => x.Name).ToList();
            var total = ordered.Count;

            var items = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<TrafficSignDTO>
            {
                Items = _mapper.Map<List<TrafficSignDTO>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<TrafficSignDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<TrafficSignDTO>(entity);
        }

        public async Task<bool> CreateAsync(TrafficSignCreateDTO dto)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var entity = _mapper.Map<TrafficSign>(dto);
            entity.Id = Guid.NewGuid();
            entity.Index = dto.Index;
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Status = 1;
            entity.Image = dto.Image;

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> CreateManyAsync(List<TrafficSignCreateDTO> dtos)
        {
            if (dtos == null || dtos.Count == 0)
                throw new ArgumentException("Danh sách biển báo không được rỗng.");

            foreach (var dto in dtos)
            {
                await CreateAsync(dto);
            }

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, TrafficSignUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy TrafficSign");

            existing.SignCategoryId = dto.SignCategoryId;
            existing.Index = dto.Index;
            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.Description = dto.Description;
            existing.VectorData = dto.VectorData;
            existing.Image = dto.Image;
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