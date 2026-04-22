using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.QuestionCategory;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class QuestionCategoryService : IQuestionCategoryService
    {
        private readonly IQuestionCategoryRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public QuestionCategoryService(
            IQuestionCategoryRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<QuestionCategoryDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var all = (await _repository.GetAllAsync(id, name, description, status, role))
                .OrderByDescending(x => x.UpdateAt ?? x.CreateAt ?? DateTime.MinValue)
                .ThenByDescending(x => x.CreateAt ?? DateTime.MinValue)
                .ThenByDescending(x => x.Id)
                .ToList();
            return _mapper.Map<List<QuestionCategoryDTO>>(all);
        }

        public async Task<PagedResult<QuestionCategoryDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, name, description, status);
            var total = items.Count;

            return new PagedResult<QuestionCategoryDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<QuestionCategoryDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<QuestionCategoryDTO>(entity);
        }

        public async Task<QuestionCategoryDTO> CreateAsync(QuestionCategoryCreateDTO dto)
        {
            var now = DateTimeHelper.GetVietnamNow();

            var entity = new QuestionCategory
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _repository.AddAsync(entity);
            return _mapper.Map<QuestionCategoryDTO>(entity);
        }

        public Task<(byte[] Content, string FileName, string ContentType)> GenerateImportTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("QuestionCategories");

            var headers = new[] { "Id", "Name", "Description", "Status", "CreateAt", "UpdateAt" };
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
                "question-category-import-template.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }

        public async Task<List<QuestionCategoryDTO>> ImportAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw ApiException.BadRequest("File không hợp lệ hoặc rỗng.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx")
                throw ApiException.BadRequest("Chỉ hỗ trợ file .xlsx");

            var items = await ParseXlsxAsync(file);
            if (items.Count == 0)
                throw ApiException.BadRequest("Không có dữ liệu hợp lệ để import.");

            var createdItems = new List<QuestionCategoryDTO>();
            foreach (var item in items)
            {
                var created = await CreateAsync(item);
                createdItems.Add(created);
            }

            return createdItems;
        }

        public async Task<(byte[] Content, string FileName, string ContentType)> ExportToExcelAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null)
        {
            var items = await GetAllAsync(id, name, description, status);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("QuestionCategories");

            var headers = new[] { "Name", "Description" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            for (int row = 0; row < items.Count; row++)
            {
                var item = items[row];
                var r = row + 2;

                worksheet.Cell(r, 1).Value = item.Id.ToString();
                worksheet.Cell(r, 2).Value = item.Name;
                worksheet.Cell(r, 3).Value = item.Description ?? string.Empty;
                worksheet.Cell(r, 4).Value = item.Status;
                worksheet.Cell(r, 5).Value = item.CreateAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
                worksheet.Cell(r, 6).Value = item.UpdateAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return (
                stream.ToArray(),
                "question-categories.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<QuestionCategoryDTO> UpdateAsync(Guid id, QuestionCategoryUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy QuestionCategory");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<QuestionCategoryDTO>(existing);
        }

        public async Task<QuestionCategoryDTO> DeleteSoftAsync(Guid id)
        {
            var entity = await _repository.GetByIdForUpdateAsync(id);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            if (entity.Status != 0 && entity.Status != 1)
                throw ApiException.BadRequest("Chỉ hỗ trợ chuyển trạng thái giữa 0 và 1.");

            entity.Status = entity.Status == 1 ? 0 : 1;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _repository.UpdateAsync(entity);
            return _mapper.Map<QuestionCategoryDTO>(entity);
        }

        public async Task<QuestionCategoryDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<QuestionCategoryDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }

        private static async Task<List<QuestionCategoryCreateDTO>> ParseXlsxAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault()
                ?? throw ApiException.BadRequest("File Excel không có worksheet.");

            var firstRow = worksheet.FirstRowUsed();
            if (firstRow == null)
                throw ApiException.BadRequest("File Excel không có header.");

            var headerMap = BuildHeaderMap(firstRow.CellsUsed().Select(c => c.GetString()).ToList());
            var result = new List<QuestionCategoryCreateDTO>();

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                if (row.CellsUsed().All(c => string.IsNullOrWhiteSpace(c.GetString())))
                    continue;

                var name = GetCellValue(row, headerMap, "Name")?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                    throw ApiException.BadRequest($"Dòng {row.RowNumber()}: Name là bắt buộc.");

                result.Add(new QuestionCategoryCreateDTO
                {
                    Name = name,
                    Description = GetCellValue(row, headerMap, "Description")?.Trim()
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

            if (!map.ContainsKey("name"))
                throw ApiException.BadRequest("Thiếu cột bắt buộc: name");

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