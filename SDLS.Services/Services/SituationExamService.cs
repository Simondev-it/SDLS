using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SituationExam;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SituationExamService : ISituationExamService
    {
        private readonly ISituationExamRepository _repository;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public SituationExamService(
            ISituationExamRepository repository,
            AppDbContext dbContext,
            IMapper mapper)
        {
            _repository = repository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PagedResult<SituationExamDTO>> GetAllAsync(
            Guid? id = null,
            string? title = null,
            string? description = null,
            bool? isRandom = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var all = await _repository.GetAllAsync(id, title, description, isRandom);
            var ordered = all.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Id).ToList();
            var total = ordered.Count;

            var items = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<SituationExamDTO>
            {
                Items = _mapper.Map<List<SituationExamDTO>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<SituationExamDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<SituationExamDTO>(entity);
        }

        public async Task<bool> CreateAsync(SituationExamCreateDTO dto)
        {
            ValidateSimulationExamList(dto.SimulationExams);

            var now = DateTime.UtcNow.ToLocalTime();
            var scenarioIds = dto.SimulationExams.Select(x => x.SimulationId).Distinct().ToList();
            var duration = await CalculateDurationAsync(scenarioIds);

            var entity = new SituationExam
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Duration = duration,
                PassScore = dto.PassScore,
                IsRandom = dto.IsRandom,
                CreateAt = now,
                UpdateAt = now,
                Status = 1,
                SimulationExams = dto.SimulationExams.Select(x => new SimulationExam
                {
                    Id = Guid.NewGuid(),
                    SituationExamId = Guid.Empty, // EF set after attach parent
                    SimulationId = x.SimulationId,
                    BaseScore = x.BaseScore,
                    CreateAt = now,
                    UpdateAt = now,
                    Status = 1
                }).ToList()
            };

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, SituationExamUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy SituationExam");

            ValidateSimulationExamList(dto.SimulationExams);

            var now = DateTime.UtcNow.ToLocalTime();

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.PassScore = dto.PassScore;
            existing.IsRandom = dto.IsRandom;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = now;

            var incomingById = dto.SimulationExams
                .Where(x => x.Id.HasValue)
                .ToDictionary(x => x.Id!.Value, x => x);

            var activeExistingChildren = existing.SimulationExams
                .Where(x => x.Status == 1)
                .ToList();

            foreach (var child in activeExistingChildren)
            {
                if (!incomingById.ContainsKey(child.Id))
                {
                    child.Status = 0;
                    child.UpdateAt = now;
                }
            }

            foreach (var item in dto.SimulationExams)
            {
                if (item.Id.HasValue)
                {
                    var child = existing.SimulationExams.FirstOrDefault(x => x.Id == item.Id.Value);
                    if (child == null)
                        throw new KeyNotFoundException($"Không tìm thấy SimulationExam với Id {item.Id.Value}");

                    child.SimulationId = item.SimulationId;
                    child.BaseScore = item.BaseScore;
                    child.Status = item.Status ?? 1;
                    child.UpdateAt = now;
                }
                else
                {
                    existing.SimulationExams.Add(new SimulationExam
                    {
                        Id = Guid.NewGuid(),
                        SituationExamId = id,
                        SimulationId = item.SimulationId,
                        BaseScore = item.BaseScore,
                        CreateAt = now,
                        UpdateAt = now,
                        Status = item.Status ?? 1
                    });
                }
            }

            var activeScenarioIds = existing.SimulationExams
                .Where(x => x.Status == 1)
                .Select(x => x.SimulationId)
                .Distinct()
                .ToList();

            existing.Duration = await CalculateDurationAsync(activeScenarioIds);

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

        private static void ValidateSimulationExamList(List<SimulationExamCreateDTO> items)
        {
            var duplicate = items
                .GroupBy(x => x.SimulationId)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate != null)
                throw new InvalidOperationException($"SimulationId bị trùng: {duplicate.Key}");
        }

        private static void ValidateSimulationExamList(List<SimulationExamUpdateDTO> items)
        {
            var duplicate = items
                .GroupBy(x => x.SimulationId)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate != null)
                throw new InvalidOperationException($"SimulationId bị trùng: {duplicate.Key}");
        }

        private async Task<int> CalculateDurationAsync(List<Guid> scenarioIds)
        {
            if (scenarioIds.Count == 0)
                return 0;

            var scenarios = await _dbContext.SimulationScenarios
                .Where(x => scenarioIds.Contains(x.Id) && x.Status == 1)
                .Select(x => new { x.Id, x.TotalTime })
                .ToListAsync();

            if (scenarios.Count != scenarioIds.Count)
                throw new KeyNotFoundException("Có SimulationScenario không tồn tại hoặc không active.");

            return scenarios.Sum(x => x.TotalTime);
        }
    }
}