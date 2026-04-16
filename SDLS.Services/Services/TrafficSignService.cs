using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.TrafficSign;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;
using System.Text;

namespace SDLS.Services.Services
{
    public class TrafficSignService : ITrafficSignService
    {
        private readonly ITrafficSignRepository _repository;
        private readonly ISignCategoryRepository _signCategoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public TrafficSignService(
            ITrafficSignRepository repository,
            ISignCategoryRepository signCategoryRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _signCategoryRepository = signCategoryRepository;
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
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<TrafficSignDTO>(entity);
        }

        public async Task<TrafficSignDTO> CreateAsync(TrafficSignCreateDTO dto)
        {
            var category = await _signCategoryRepository.GetByIdAsync(dto.SignCategoryId);
            if (category == null)
                throw ApiException.BadRequest($"Không tìm thấy SignCategory với ID {dto.SignCategoryId}");

            var now = DateTimeHelper.GetVietnamNow();

            var entity = _mapper.Map<TrafficSign>(dto);
            entity.Id = Guid.NewGuid();
            entity.Index = dto.Index;
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Status = 1;
            entity.Image = dto.Image;

            await _repository.AddAsync(entity);
            return _mapper.Map<TrafficSignDTO>(entity);
        }

        public async Task<List<TrafficSignDTO>> CreateManyAsync(List<TrafficSignCreateDTO> dtos)
        {
            if (dtos == null || dtos.Count == 0)
                throw ApiException.BadRequest("Danh sách biển báo không được rỗng.");

            var createdItems = new List<TrafficSignDTO>();

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
            var worksheet = workbook.Worksheets.Add("TrafficSigns");

            var headers = new[]
            {
                "SignCategoryId",
                "Index",
                "Name",
                "Code",
                "Description",
                "VectorData",
                "Image"
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
                "traffic-sign-import-template.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }

        public async Task<List<TrafficSignDTO>> ImportAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw ApiException.BadRequest("File không hợp lệ hoặc rỗng.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            List<TrafficSignCreateDTO> items = extension switch
            {
                ".xlsx" => await ParseXlsxAsync(file),
                ".csv" => await ParseCsvAsync(file),
                _ => throw ApiException.BadRequest("Chỉ hỗ trợ file .csv hoặc .xlsx")
            };

            if (items.Count == 0)
                throw ApiException.BadRequest("Không có dữ liệu hợp lệ để import.");

            return await CreateManyAsync(items);
        }

        public async Task<TrafficSignDTO> UpdateAsync(Guid id, TrafficSignUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy TrafficSign");

            var category = await _signCategoryRepository.GetByIdAsync(dto.SignCategoryId);
            if (category == null)
                throw ApiException.BadRequest($"Không tìm thấy SignCategory với ID {dto.SignCategoryId}");

            existing.SignCategoryId = dto.SignCategoryId;
            existing.Index = dto.Index;
            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.Description = dto.Description;
            existing.VectorData = dto.VectorData;
            existing.Image = dto.Image;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<TrafficSignDTO>(existing);
        }

        public async Task<TrafficSignDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            entity.Status = entity.Status == 0 ? 1 : 0;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<TrafficSignDTO>(entity);
        }

        public async Task<TrafficSignDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<TrafficSignDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }

        private async Task<List<TrafficSignCreateDTO>> ParseXlsxAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault()
                ?? throw ApiException.BadRequest("File Excel không có worksheet.");

            var firstRow = worksheet.FirstRowUsed();
            if (firstRow == null)
                throw ApiException.BadRequest("File Excel không có header.");

            var headerMap = BuildHeaderMap(firstRow.CellsUsed().Select(c => c.GetString()).ToList());
            var result = new List<TrafficSignCreateDTO>();

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                if (row.CellsUsed().All(c => string.IsNullOrWhiteSpace(c.GetString())))
                    continue;

                result.Add(BuildTrafficSignDto(key => GetCellValue(row, headerMap, key), row.RowNumber()));
            }

            return result;
        }

        private async Task<List<TrafficSignCreateDTO>> ParseCsvAsync(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, true);
            var headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
                throw ApiException.BadRequest("File CSV không có header.");

            var headers = ParseCsvLine(headerLine);
            var headerMap = BuildHeaderMap(headers);

            var result = new List<TrafficSignCreateDTO>();
            var rowNumber = 1;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                rowNumber++;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = ParseCsvLine(line);
                if (values.All(string.IsNullOrWhiteSpace))
                    continue;

                result.Add(BuildTrafficSignDto(key => GetCsvValue(values, headerMap, key), rowNumber));
            }

            return result;
        }

        private static TrafficSignCreateDTO BuildTrafficSignDto(Func<string, string?> getValue, int rowNumber)
        {
            var signCategoryRaw = getValue("SignCategoryId");
            if (!Guid.TryParse(signCategoryRaw, out var signCategoryId) || signCategoryId == Guid.Empty)
                throw ApiException.BadRequest($"Dòng {rowNumber}: SignCategoryId không hợp lệ.");

            var name = getValue("Name")?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw ApiException.BadRequest($"Dòng {rowNumber}: Name là bắt buộc.");

            var code = getValue("Code")?.Trim();
            if (string.IsNullOrWhiteSpace(code))
                throw ApiException.BadRequest($"Dòng {rowNumber}: Code là bắt buộc.");

            int? index = null;
            var indexRaw = getValue("Index")?.Trim();
            if (!string.IsNullOrWhiteSpace(indexRaw))
            {
                if (!int.TryParse(indexRaw, out var parsedIndex) || parsedIndex <= 0)
                    throw ApiException.BadRequest($"Dòng {rowNumber}: Index không hợp lệ.");

                index = parsedIndex;
            }

            return new TrafficSignCreateDTO
            {
                SignCategoryId = signCategoryId,
                Index = index,
                Name = name,
                Code = code,
                Description = getValue("Description")?.Trim(),
                VectorData = getValue("VectorData")?.Trim(),
                Image = getValue("Image")?.Trim()
            };
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

            var required = new[] { "signcategoryid", "name", "code" };
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

        private static string? GetCsvValue(List<string> values, Dictionary<string, int> headerMap, string key)
        {
            var normalizedKey = NormalizeHeader(key);
            if (!headerMap.TryGetValue(normalizedKey, out var index))
                return null;

            return index < values.Count ? values[index] : null;
        }

        private static string NormalizeHeader(string? value)
        {
            return (value ?? string.Empty)
                .Trim()
                .Replace("_", string.Empty)
                .Replace(" ", string.Empty)
                .ToLowerInvariant();
        }

        private static List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var ch = line[i];

                if (ch == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (ch == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(ch);
                }
            }

            result.Add(current.ToString());
            return result;
        }
    }
}