using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.SimulationSession;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SimulationSessionService : ISimulationSessionService
    {
        private readonly ISimulationSessionRepository _repository;
        private readonly ISimulationExamRepository _simulationExamRepository;
        private readonly ISituationExamRepository _situationExamRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public SimulationSessionService(
            ISimulationSessionRepository repository,
            ISimulationExamRepository simulationExamRepository,
            ISituationExamRepository situationExamRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _simulationExamRepository = simulationExamRepository;
            _situationExamRepository = situationExamRepository;
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
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<SimulationSessionDTO>(entity);
        }

        public async Task<SimulationSessionDTO> CreateAsync(SimulationSessionCreateDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.SituationExamId == Guid.Empty)
                throw ApiException.BadRequest("SituationExamId không được rỗng.");

            var situationExamExists = await _situationExamRepository.GetByIdAsync(dto.SituationExamId);
            if (situationExamExists == null)
                throw ApiException.BadRequest("SituationExamId không hợp lệ.");

            ValidateDetailCreateList(dto.SimulationSessionDetails);

            var passScore = await _situationExamRepository.GetPassScoreAsync(dto.SituationExamId);
            await _simulationExamRepository.ValidateSimulationExamIdsAsync(
                dto.SituationExamId,
                dto.SimulationSessionDetails.Select(x => x.SimulationExamId).Distinct().ToList());

            var now = DateTimeHelper.GetVietnamNow();
            var totalScore = dto.SimulationSessionDetails.Sum(x => x.Score ?? 0);
            var totalDuration = dto.SimulationSessionDetails.Sum(x => x.DurationSecond ?? 0d);

            var entity = new SimulationSession
            {
                Id = Guid.NewGuid(),
                SituationExamId = dto.SituationExamId,
                UserId = currentUserId,
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
            return _mapper.Map<SimulationSessionDTO>(entity);
        }

        public async Task<SimulationSessionDTO> UpdateAsync(Guid id, SimulationSessionUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy SimulationSession");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.SituationExamId == Guid.Empty)
                throw ApiException.BadRequest("SituationExamId không được rỗng.");

            var situationExamExists = await _situationExamRepository.GetByIdAsync(dto.SituationExamId);
            if (situationExamExists == null)
                throw ApiException.BadRequest("SituationExamId không hợp lệ.");

            ValidateDetailUpdateList(dto.SimulationSessionDetails);

            var passScore = await _situationExamRepository.GetPassScoreAsync(dto.SituationExamId);
            await _simulationExamRepository.ValidateSimulationExamIdsAsync(
                dto.SituationExamId,
                dto.SimulationSessionDetails.Select(x => x.SimulationExamId).Distinct().ToList());

            var now = DateTimeHelper.GetVietnamNow();

            existing.SituationExamId = dto.SituationExamId;
            existing.UserId = currentUserId;
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
                        throw ApiException.NotFound($"Không tìm thấy SimulationSessionDetail với Id {item.Id.Value}");

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
            var totalDuration = activeDetails.Sum(x => x.DurationSecond ?? 0d);

            existing.TotalScore = totalScore;
            existing.TotalDuration = totalDuration;
            existing.IsPassed = totalScore >= passScore;

            await _repository.UpdateAsync(existing);
            return _mapper.Map<SimulationSessionDTO>(existing);
        }

        public async Task<SimulationSessionDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            entity.Status = 0;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<SimulationSessionDTO>(entity);
        }

        public async Task<SimulationSessionDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<SimulationSessionDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }

        private static void ValidateDetailCreateList(List<SimulationSessionDetailCreateDTO> items)
        {
            if (items == null || items.Count == 0)
                throw ApiException.BadRequest("SimulationSessionDetails không được rỗng.");

            var duplicate = items
                .GroupBy(x => x.SimulationExamId)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate != null)
                throw ApiException.Conflict($"SimulationExamId bị trùng: {duplicate.Key}");
        }

        private static void ValidateDetailUpdateList(List<SimulationSessionDetailUpdateDTO> items)
        {
            if (items == null || items.Count == 0)
                throw ApiException.BadRequest("SimulationSessionDetails không được rỗng.");

            var duplicate = items
                .GroupBy(x => x.SimulationExamId)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate != null)
                throw ApiException.Conflict($"SimulationExamId bị trùng: {duplicate.Key}");
        }
    }
}