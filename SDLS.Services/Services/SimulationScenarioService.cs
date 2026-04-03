using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationScenario;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SimulationScenarioService : ISimulationScenarioService
    {
        private readonly ISimulationScenarioRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public SimulationScenarioService(
            ISimulationScenarioRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<PagedResult<SimulationScenarioDTO>> GetAllAsync(
            Guid? simulationCategoryId = null,
            Guid? simulationChapterId = null,
            Guid? simulationDifficultyLevelId = null,
            string? name = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var filtered = await _repository.GetAllAsync(
                simulationCategoryId, simulationChapterId, simulationDifficultyLevelId, name, status, role);

            var total = filtered.Count();

            var pagedEntities = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<SimulationScenarioDTO>
            {
                Items = _mapper.Map<List<SimulationScenarioDTO>>(pagedEntities),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<SimulationScenarioDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<SimulationScenarioDTO>(entity);
        }

        public async Task<bool> CreateAsync(SimulationScenarioCreateDTO dto)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var entity = _mapper.Map<SimulationScenario>(dto);
            entity.Id = Guid.NewGuid();
            entity.Index = dto.Index;
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Status = 1;

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> CreateManyAsync(List<SimulationScenarioCreateDTO> dtos)
        {
            if (dtos == null || dtos.Count == 0)
                throw new ArgumentException("Danh sách tình huống mô phỏng không được rỗng.");

            foreach (var dto in dtos)
            {
                await CreateAsync(dto);
            }

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, SimulationScenarioUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy simulation scenario");

            existing.SimulationChapterId = dto.SimulationChapterId;
            existing.SimulationCategoryId = dto.SimulationCategoryId;
            existing.SimulationDifficultyLevelId = dto.SimulationDifficultyLevelId;
            existing.Index = dto.Index;
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Video = dto.Video;
            existing.TotalTime = dto.TotalTime;
            existing.StartPoint = dto.StartPoint;
            existing.EndPoint = dto.EndPoint;
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