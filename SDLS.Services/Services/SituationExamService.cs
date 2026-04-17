using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.SituationExam;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SituationExamService : ISituationExamService
    {
        private readonly ISituationExamRepository _repository;
        private readonly ISimulationScenarioRepository _simulationScenarioRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public SituationExamService(
            ISituationExamRepository repository,
            ISimulationScenarioRepository simulationScenarioRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _simulationScenarioRepository = simulationScenarioRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<PagedResult<SituationExamDTO>> GetAllAsync(
            Guid? id = null,
            string? title = null,
            string? description = null,
            bool? isRandom = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var all = await _repository.GetAllAsync(id, title, description, isRandom, status, role);

            var ordered = all.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Id).ToList();
            var total = ordered.Count;

            var items = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mappedItems = _mapper.Map<List<SituationExamDTO>>(items);
            mappedItems.ForEach(ApplyGetRounding);

            return new PagedResult<SituationExamDTO>
            {
                Items = mappedItems,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<SituationExamDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<SituationExamDTO>(entity);
            ApplyGetRounding(result);
            return result;
        }

        public async Task<SituationExamDTO> CreateAsync(SituationExamCreateDTO dto)
        {
            ValidateSimulationExamList(dto.SimulationExams);

            var now = DateTimeHelper.GetVietnamNow();
            var scenarioIds = dto.SimulationExams.Select(x => x.SimulationId).Distinct().ToList();
            var duration = await _simulationScenarioRepository.CalculateDurationAsync(scenarioIds);

            foreach (var scenarioId in scenarioIds)
            {
                var exists = await _simulationScenarioRepository.GetByIdAsync(scenarioId);
                if (exists == null)
                    throw ApiException.NotFound($"Không tìm thấy SimulationScenario với ID {scenarioId}");
            }

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
            return _mapper.Map<SituationExamDTO>(entity);
        }

        public async Task<SituationExamDTO> UpdateAsync(Guid id, SituationExamUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy SituationExam");

            ValidateSimulationExamList(dto.SimulationExams);

            var now = DateTimeHelper.GetVietnamNow();

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
                        throw ApiException.NotFound($"Không tìm thấy SimulationExam với Id {item.Id.Value}");

                    // Kiểm tra tồn tại của SimulationScenario trước khi cập nhật
                    var exists = await _simulationScenarioRepository.GetByIdAsync(item.SimulationId);
                    if (exists == null)
                        throw ApiException.NotFound($"Không tìm thấy SimulationScenario với ID {item.SimulationId}");
                    

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

            existing.Duration = await _simulationScenarioRepository.CalculateDurationAsync(activeScenarioIds);

            await _repository.UpdateAsync(existing);
            return _mapper.Map<SituationExamDTO>(existing);
        }

        public async Task<SituationExamDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            entity.Status = 0;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<SituationExamDTO>(entity);
        }

        public async Task<SituationExamDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<SituationExamDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }

        private static void ValidateSimulationExamList(List<SimulationExamCreateDTO> items)
        {
            var duplicate = items
                .GroupBy(x => x.SimulationId)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate != null)
                throw ApiException.Conflict($"SimulationId bị trùng: {duplicate.Key}");
        }

        private static void ValidateSimulationExamList(List<SimulationExamUpdateDTO> items)
        {
            var duplicate = items
                .GroupBy(x => x.SimulationId)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate != null)
                throw ApiException.Conflict($"SimulationId bị trùng: {duplicate.Key}");
        }

        private static void ApplyGetRounding(SituationExamDTO dto)
        {
            dto.Duration = Round2(dto.Duration);

            foreach (var simulationExam in dto.SimulationExams)
            {
                if (simulationExam.Simulation == null)
                    continue;

                simulationExam.Simulation.TotalTime = Round2(simulationExam.Simulation.TotalTime);
                simulationExam.Simulation.StartPoint = Round2(simulationExam.Simulation.StartPoint);
                simulationExam.Simulation.EndPoint = Round2(simulationExam.Simulation.EndPoint);
            }
        }

        private static double? Round2(double? value)
            => value.HasValue
                ? Math.Round(value.Value, 2, MidpointRounding.AwayFromZero)
                : null;

        private static double Round2(double value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}