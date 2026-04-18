using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.SimulationScenario;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;
using System.Globalization;

namespace SDLS.Services.Services
{
    public class SimulationScenarioService : ISimulationScenarioService
    {
        private readonly ISimulationScenarioRepository _repository;
        private readonly ISimulationChapterRepository _simulationChapterRepository;
        private readonly ISimulationCategoryRepository _simulationCategoryRepository;
        private readonly ISimulationDifficultyLevelRepository _simulationDifficultyLevelRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public SimulationScenarioService(
            ISimulationScenarioRepository repository,
            ISimulationChapterRepository simulationChapterRepository,
            ISimulationCategoryRepository simulationCategoryRepository,
            ISimulationDifficultyLevelRepository simulationDifficultyLevelRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _simulationChapterRepository = simulationChapterRepository;
            _simulationCategoryRepository = simulationCategoryRepository;
            _simulationDifficultyLevelRepository = simulationDifficultyLevelRepository;
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

            var mappedItems = _mapper.Map<List<SimulationScenarioDTO>>(pagedEntities);
            mappedItems.ForEach(ApplyGetRounding);

            return new PagedResult<SimulationScenarioDTO>
            {
                Items = mappedItems,
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
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<SimulationScenarioDTO>(entity);
            ApplyGetRounding(result);
            return result;
        }

        public async Task<SimulationScenarioDTO> CreateAsync(SimulationScenarioCreateDTO dto)
        {
            var chapter = await _simulationChapterRepository.GetByIdAsync(dto.SimulationChapterId);
            if (chapter == null)
                throw ApiException.BadRequest("Simulation chapter không tồn tại.");

            var category = await _simulationCategoryRepository.GetByIdAsync(dto.SimulationCategoryId);
            if (category == null)
                throw ApiException.BadRequest("Simulation category không tồn tại.");

            var difficultyLevel = await _simulationDifficultyLevelRepository.GetByIdAsync(dto.SimulationDifficultyLevelId);
            if (difficultyLevel == null)
                throw ApiException.BadRequest("Simulation difficulty level không tồn tại.");


            var now = DateTimeHelper.GetVietnamNow();

            var entity = _mapper.Map<SimulationScenario>(dto);
            entity.Id = Guid.NewGuid();
            entity.Index = dto.Index;
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Status = 1;

            await _repository.AddAsync(entity);
            return _mapper.Map<SimulationScenarioDTO>(entity);
        }

        public async Task<List<SimulationScenarioDTO>> CreateManyAsync(List<SimulationScenarioCreateDTO> dtos)
        {
            if (dtos == null || dtos.Count == 0)
                throw ApiException.BadRequest("Danh sách tình huống mô phỏng không được rỗng.");

            var createdItems = new List<SimulationScenarioDTO>();

            foreach (var dto in dtos)
            {
                var created = await CreateAsync(dto);
                createdItems.Add(created);
            }

            return createdItems;
        }

        public Task<(byte[] Content, string FileName, string ContentType)> GenerateImportTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("SimulationScenarios");

            var headers = new[]
            {
                "SimulationChapterId",
                "SimulationCategoryId",
                "SimulationDifficultyLevelId",
                "Index",
                "Name",
                "Description",
                "Video",
                "TotalTime",
                "StartPoint",
                "EndPoint"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return Task.FromResult((
                stream.ToArray(),
                "simulation-scenario-import-template.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }

        public async Task<List<SimulationScenarioDTO>> ImportAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw ApiException.BadRequest("File không hợp lệ hoặc rỗng.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx")
                throw ApiException.BadRequest("Chỉ hỗ trợ file .xlsx");

            var items = await ParseXlsxAsync(file);
            if (items.Count == 0)
                throw ApiException.BadRequest("Không có dữ liệu hợp lệ để import.");

            return await CreateManyAsync(items);
        }

        public async Task<SimulationScenarioDTO> UpdateAsync(Guid id, SimulationScenarioUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy simulation scenario");

            var chapter = await _simulationChapterRepository.GetByIdAsync(dto.SimulationChapterId);
            if (chapter == null)
                throw ApiException.BadRequest("Simulation chapter không tồn tại.");

            var category = await _simulationCategoryRepository.GetByIdAsync(dto.SimulationCategoryId);
            if (category == null)
                throw ApiException.BadRequest("Simulation category không tồn tại.");

            var difficultyLevel = await _simulationDifficultyLevelRepository.GetByIdAsync(dto.SimulationDifficultyLevelId);
            if (difficultyLevel == null)
                throw ApiException.BadRequest("Simulation difficulty level không tồn tại.");

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
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<SimulationScenarioDTO>(existing);
        }

        public async Task<SimulationScenarioDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            entity.Status = 0;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<SimulationScenarioDTO>(entity);
        }

        public async Task<SimulationScenarioDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<SimulationScenarioDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }

        private async Task<List<SimulationScenarioCreateDTO>> ParseXlsxAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault()
                ?? throw ApiException.BadRequest("File Excel không có worksheet.");

            var firstRow = worksheet.FirstRowUsed();
            if (firstRow == null)
                throw ApiException.BadRequest("File Excel không có header.");

            var headerMap = BuildHeaderMap(firstRow.CellsUsed().Select(c => c.GetString()).ToList());
            var result = new List<SimulationScenarioCreateDTO>();

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                if (row.CellsUsed().All(c => string.IsNullOrWhiteSpace(c.GetString())))
                    continue;

                result.Add(BuildSimulationScenarioDto(key => GetCellValue(row, headerMap, key), row.RowNumber()));
            }

            return result;
        }

        private static SimulationScenarioCreateDTO BuildSimulationScenarioDto(Func<string, string?> getValue, int rowNumber)
        {
            var simulationChapterRaw = getValue("SimulationChapterId");
            if (!Guid.TryParse(simulationChapterRaw, out var simulationChapterId) || simulationChapterId == Guid.Empty)
                throw ApiException.BadRequest($"Dòng {rowNumber}: SimulationChapterId không hợp lệ.");

            var simulationCategoryRaw = getValue("SimulationCategoryId");
            if (!Guid.TryParse(simulationCategoryRaw, out var simulationCategoryId) || simulationCategoryId == Guid.Empty)
                throw ApiException.BadRequest($"Dòng {rowNumber}: SimulationCategoryId không hợp lệ.");

            var simulationDifficultyRaw = getValue("SimulationDifficultyLevelId");
            if (!Guid.TryParse(simulationDifficultyRaw, out var simulationDifficultyLevelId) || simulationDifficultyLevelId == Guid.Empty)
                throw ApiException.BadRequest($"Dòng {rowNumber}: SimulationDifficultyLevelId không hợp lệ.");

            var name = getValue("Name")?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw ApiException.BadRequest($"Dòng {rowNumber}: Name là bắt buộc.");

            var video = getValue("Video")?.Trim();
            if (string.IsNullOrWhiteSpace(video))
                throw ApiException.BadRequest($"Dòng {rowNumber}: Video là bắt buộc.");

            int? index = null;
            var indexRaw = getValue("Index")?.Trim();
            if (!string.IsNullOrWhiteSpace(indexRaw))
            {
                if (!int.TryParse(indexRaw, out var parsedIndex) || parsedIndex <= 0)
                    throw ApiException.BadRequest($"Dòng {rowNumber}: Index không hợp lệ.");

                index = parsedIndex;
            }

            var totalTime = ParseDouble(getValue("TotalTime"), rowNumber, "TotalTime");
            var startPoint = ParseDouble(getValue("StartPoint"), rowNumber, "StartPoint");
            var endPoint = ParseDouble(getValue("EndPoint"), rowNumber, "EndPoint");

            return new SimulationScenarioCreateDTO
            {
                SimulationChapterId = simulationChapterId,
                SimulationCategoryId = simulationCategoryId,
                SimulationDifficultyLevelId = simulationDifficultyLevelId,
                Index = index,
                Name = name,
                Description = getValue("Description")?.Trim(),
                Video = video,
                TotalTime = totalTime,
                StartPoint = startPoint,
                EndPoint = endPoint
            };
        }

        private static double ParseDouble(string? raw, int rowNumber, string fieldName)
        {
            var value = raw?.Trim();
            if (string.IsNullOrWhiteSpace(value))
                throw ApiException.BadRequest($"Dòng {rowNumber}: {fieldName} là bắt buộc.");

            if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var invariantParsed))
                return invariantParsed;

            if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out var currentParsed))
                return currentParsed;

            throw ApiException.BadRequest($"Dòng {rowNumber}: {fieldName} không hợp lệ.");
        }

        private static Dictionary<string, int> BuildHeaderMap(List<string> headers)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < headers.Count; i++)
            {
                var key = NormalizeHeader(headers[i]);
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                map[key] = i;
            }

            var required = new[]
            {
                "simulationchapterid",
                "simulationcategoryid",
                "simulationdifficultylevelid",
                "name",
                "video",
                "totaltime",
                "startpoint",
                "endpoint"
            };

            foreach (var req in required)
            {
                if (!map.ContainsKey(req))
                    throw ApiException.BadRequest($"Thiếu cột bắt buộc: {req}");
            }

            return map;
        }

        private static string? GetCellValue(IXLRow row, Dictionary<string, int> headerMap, string key)
        {
            var normalizedKey = NormalizeHeader(key);
            if (!headerMap.TryGetValue(normalizedKey, out var index))
                return null;

            return row.Cell(index + 1).GetString();
        }

        private static string NormalizeHeader(string? value)
        {
            return (value ?? string.Empty)
                .Trim()
                .Replace("_", string.Empty)
                .Replace(" ", string.Empty)
                .ToLowerInvariant();
        }

        private static void ApplyGetRounding(SimulationScenarioDTO dto)
        {
            dto.TotalTime = Round2(dto.TotalTime);
            dto.StartPoint = Round2(dto.StartPoint);
            dto.EndPoint = Round2(dto.EndPoint);
        }

        private static double Round2(double value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}