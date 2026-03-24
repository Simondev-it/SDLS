using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationScenario;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SimulationScenarioService : ISimulationScenarioService
    {
        private readonly ISimulationScenarioRepository _repository;
        private readonly IMapper _mapper;

        public SimulationScenarioService(ISimulationScenarioRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<SimulationScenarioDTO>> GetAllAsync(
            Guid? simulationCategoryId = null,
            Guid? simulationChapterId = null,
            Guid? simulationDifficultyLevelId = null,
            string? name = null,
            int page = 1,
            int pageSize = 20)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable();

            if (simulationCategoryId.HasValue)
                filtered = filtered.Where(x => x.SimulationCategoryId == simulationCategoryId.Value);

            if (simulationChapterId.HasValue)
                filtered = filtered.Where(x => x.SimulationChapterId == simulationChapterId.Value);

            if (simulationDifficultyLevelId.HasValue)
                filtered = filtered.Where(x => x.SimulationDifficultyLevelId == simulationDifficultyLevelId.Value);

            if (!string.IsNullOrWhiteSpace(name))
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.Name)
                    && x.Name.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase));

            var total = filtered.Count();

            var pagedEntities = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedDtos = _mapper.Map<List<SimulationScenarioDTO>>(pagedEntities);

            return new PagedResult<SimulationScenarioDTO>
            {
                Items = pagedDtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<SimulationScenarioDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<SimulationScenarioDTO>(entity);
        }

        public async Task<bool> CreateAsync(SimulationScenarioCreateDTO dto)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var entity = _mapper.Map<SimulationScenario>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Status = 1;

            await _repository.AddAsync(entity);
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

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}