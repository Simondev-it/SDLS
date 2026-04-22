using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.Tag;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public TagService(ITagRepository repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<TagDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            string? colorCode = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = (await _repository.GetAllAsync(id, name, description, colorCode, status, role))
                .OrderByDescending(x => x.UpdateAt ?? x.CreateAt ?? DateTime.MinValue)
                .ThenByDescending(x => x.CreateAt ?? DateTime.MinValue)
                .ThenByDescending(x => x.Id)
                .ToList();
            return _mapper.Map<List<TagDTO>>(entities);
        }

        public async Task<PagedResult<TagDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            string? colorCode = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, name, description, colorCode, status);
            var total = items.Count;

            return new PagedResult<TagDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<TagDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<TagDTO>(entity);
        }

        public async Task<TagDTO> CreateAsync(TagCreateDTO dto)
        {
            var now = DateTimeHelper.GetVietnamNow();

            var entity = new Tag
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                ColorCode = dto.ColorCode,
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _repository.AddAsync(entity);
            return _mapper.Map<TagDTO>(entity);
        }

        public Task<(byte[] Content, string FileName, string ContentType)> GenerateImportTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Tags");

            var headers = new[] { "Name", "Description", "ColorCode" };

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
                "tag-import-template.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }

        public async Task<List<TagDTO>> ImportAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw ApiException.BadRequest("File không hợp lệ hoặc rỗng.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx")
                throw ApiException.BadRequest("Chỉ hỗ trợ file .xlsx");

            var items = await ParseXlsxAsync(file);
            if (items.Count == 0)
                throw ApiException.BadRequest("Không có dữ liệu hợp lệ để import.");

            var createdItems = new List<TagDTO>();
            foreach (var item in items)
            {
                var created = await CreateAsync(item);
                createdItems.Add(created);
            }

            return createdItems;
        }

        public async Task<TagDTO> UpdateAsync(Guid id, TagUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy Tag");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.ColorCode = dto.ColorCode;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<TagDTO>(existing);
        }

        public async Task<TagDTO> DeleteSoftAsync(Guid id)
        {
            var entity = await _repository.GetByIdForUpdateAsync(id);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            if (entity.Status != 0 && entity.Status != 1)
                throw ApiException.BadRequest("Chỉ hỗ trợ chuyển trạng thái giữa 0 và 1.");

            entity.Status = entity.Status == 1 ? 0 : 1;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _repository.UpdateAsync(entity);
            return _mapper.Map<TagDTO>(entity);
        }

        public async Task<TagDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<TagDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }

        private static async Task<List<TagCreateDTO>> ParseXlsxAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault()
                ?? throw ApiException.BadRequest("File Excel không có worksheet.");

            var firstRow = worksheet.FirstRowUsed();
            if (firstRow == null)
                throw ApiException.BadRequest("File Excel không có header.");

            var headerMap = BuildHeaderMap(firstRow.CellsUsed().Select(c => c.GetString()).ToList());
            var result = new List<TagCreateDTO>();

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                if (row.CellsUsed().All(c => string.IsNullOrWhiteSpace(c.GetString())))
                    continue;

                var name = GetCellValue(row, headerMap, "Name")?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                    throw ApiException.BadRequest($"Dòng {row.RowNumber()}: Name là bắt buộc.");

                var colorCode = GetCellValue(row, headerMap, "ColorCode")?.Trim();
                if (string.IsNullOrWhiteSpace(colorCode))
                    throw ApiException.BadRequest($"Dòng {row.RowNumber()}: ColorCode là bắt buộc.");

                result.Add(new TagCreateDTO
                {
                    Name = name,
                    Description = GetCellValue(row, headerMap, "Description")?.Trim(),
                    ColorCode = colorCode
                });
            }

            return result;
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

            var required = new[] { "name", "colorcode" };
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
    }
}