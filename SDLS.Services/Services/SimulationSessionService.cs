using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationSession;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SimulationSessionService : ISimulationSessionService
    {
        private readonly ISimulationSessionRepository _repository;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public SimulationSessionService(
            ISimulationSessionRepository repository,
            AppDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<PagedResult<SimulationSessionDTO>> GetAllAsync(
            Guid? id = null,
            Guid? situationExamId = null,
            Guid? userId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var all = await _repository.GetAllAsync(id, situationExamId, userId, status, role);
            var ordered = all.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Id).ToList();
            var total = ordered.Count;

            var items = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<SimulationSessionDTO>
            {
                Items = _mapper.Map<List<SimulationSessionDTO>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<SimulationSessionDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<SimulationSessionDTO>(entity);
        }

        public async Task<bool> CreateAsync(SimulationSessionCreateDTO dto)
        {
            if (dto.SituationExamId == Guid.Empty || dto.UserId == Guid.Empty)
                throw new ArgumentException("SituationExamId và UserId không được rỗng.");

            ValidateDetailCreateList(dto.SimulationSessionDetails);

            var passScore = await GetPassScoreAsync(dto.SituationExamId);
            await ValidateSimulationExamIdsAsync(
                dto.SituationExamId,
                dto.SimulationSessionDetails.Select(x => x.SimulationExamId).Distinct().ToList());

            var now = DateTime.UtcNow.ToLocalTime();
            var totalScore = dto.SimulationSessionDetails.Sum(x => x.Score ?? 0);
            var totalDuration = dto.SimulationSessionDetails.Sum(x => x.DurationSecond ?? 0);

            var entity = new SimulationSession
            {
                Id = Guid.NewGuid(),
                SituationExamId = dto.SituationExamId,
                UserId = dto.UserId,
                TotalScore = totalScore,
                TotalDuration = totalDuration,
                IsPassed = totalScore >= passScore,
                CreateAt = now,
                UpdateAt = now,
                Status = 1,
                SimulationSessionDetails = dto.SimulationSessionDetails.Select(x => new SimulationSessionDetail
                {
                    Id = Guid.NewGuid(),
                    SimulationExamId = x.SimulationExamId,
                    DurationSecond = x.DurationSecond,
                    Score = x.Score,
                    CreateAt = now,
                    UpdateAt = now,
                    Status = 1
                }).ToList()
            };

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, SimulationSessionUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy SimulationSession");

            if (dto.SituationExamId == Guid.Empty || dto.UserId == Guid.Empty)
                throw new ArgumentException("SituationExamId và UserId không được rỗng.");

            ValidateDetailUpdateList(dto.SimulationSessionDetails);

            var passScore = await GetPassScoreAsync(dto.SituationExamId);
            await ValidateSimulationExamIdsAsync(
                dto.SituationExamId,
                dto.SimulationSessionDetails.Select(x => x.SimulationExamId).Distinct().ToList());

            var now = DateTime.UtcNow.ToLocalTime();

            existing.SituationExamId = dto.SituationExamId;
            existing.UserId = dto.UserId;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = now;

            var incomingById = dto.SimulationSessionDetails
                .Where(x => x.Id.HasValue)
                .ToDictionary(x => x.Id!.Value, x => x);

            var activeExisting = existing.SimulationSessionDetails
                .Where(x => x.Status == 1)
                .ToList();

            foreach (var child in activeExisting)
            {
                if (!incomingById.ContainsKey(child.Id))
                {
                    child.Status = 0;
                    child.UpdateAt = now;
                }
            }

            foreach (var item in dto.SimulationSessionDetails)
            {
                if (item.Id.HasValue)
                {
                    var child = existing.SimulationSessionDetails.FirstOrDefault(x => x.Id == item.Id.Value);
                    if (child == null)
                        throw new KeyNotFoundException($"Không tìm thấy SimulationSessionDetail với Id {item.Id.Value}");

                    child.SimulationExamId = item.SimulationExamId;
                    child.DurationSecond = item.DurationSecond;
                    child.Score = item.Score;
                    child.Status = item.Status ?? 1;
                    child.UpdateAt = now;
                }
                else
                {
                    existing.SimulationSessionDetails.Add(new SimulationSessionDetail
                    {
                        Id = Guid.NewGuid(),
                        SimulationSessionId = id,
                        SimulationExamId = item.SimulationExamId,
                        DurationSecond = item.DurationSecond,
                        Score = item.Score,
                        CreateAt = now,
                        UpdateAt = now,
                        Status = item.Status ?? 1
                    });
                }
            }

            var activeDetails = existing.SimulationSessionDetails
                .Where(x => x.Status == 1)
                .ToList();

            var totalScore = activeDetails.Sum(x => x.Score ?? 0);
            var totalDuration = activeDetails.Sum(x => x.DurationSecond ?? 0);

            existing.TotalScore = totalScore;
            existing.TotalDuration = totalDuration;
            existing.IsPassed = totalScore >= passScore;

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

        private static void ValidateDetailCreateList(List<SimulationSessionDetailCreateDTO> items)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("SimulationSessionDetails không được rỗng.");

            var duplicate = items
                .GroupBy(x => x.SimulationExamId)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate != null)
                throw new InvalidOperationException($"SimulationExamId bị trùng: {duplicate.Key}");
        }

        private static void ValidateDetailUpdateList(List<SimulationSessionDetailUpdateDTO> items)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("SimulationSessionDetails không được rỗng.");

            var duplicate = items
                .GroupBy(x => x.SimulationExamId)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate != null)
                throw new InvalidOperationException($"SimulationExamId bị trùng: {duplicate.Key}");
        }

        private async Task<int> GetPassScoreAsync(Guid situationExamId)
        {
            var exam = await _dbContext.SituationExams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == situationExamId && x.Status == 1);

            if (exam == null)
                throw new KeyNotFoundException("Không tìm thấy SituationExam.");

            return exam.PassScore ?? 0;
        }

        private async Task ValidateSimulationExamIdsAsync(Guid situationExamId, List<Guid> simulationExamIds)
        {
            if (simulationExamIds.Count == 0)
                throw new ArgumentException("SimulationSessionDetails không hợp lệ.");

            var validIds = await _dbContext.SimulationExams
                .Where(x => x.Status == 1
                    && x.SituationExamId == situationExamId
                    && simulationExamIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (validIds.Count != simulationExamIds.Count)
                throw new KeyNotFoundException("Có SimulationExam không tồn tại, không active hoặc không thuộc SituationExam.");
        }
    }
}