using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationDifficultyLevel;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SimulationDifficultyLevelService : ISimulationDifficultyLevelService
    {
        private readonly ISimulationDifficultyLevelRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public SimulationDifficultyLevelService(
            ISimulationDifficultyLevelRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<SimulationDifficultyLevelDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetAllAsync(id, name, description, status, role);
            return _mapper.Map<List<SimulationDifficultyLevelDTO>>(entities);
        }

        public async Task<PagedResult<SimulationDifficultyLevelDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, name, description, status);
            var total = items.Count;

            return new PagedResult<SimulationDifficultyLevelDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<SimulationDifficultyLevelDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<SimulationDifficultyLevelDTO>(entity);
        }

        public async Task<bool> CreateAsync(SimulationDifficultyLevelCreateDTO dto)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var entity = new SimulationDifficultyLevel
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

        public async Task<bool> UpdateAsync(Guid id, SimulationDifficultyLevelUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy SimulationDifficultyLevel");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            return true;
        }

        public async Task<bool> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteHardAsync(id);
            return true;
        }
    }
}