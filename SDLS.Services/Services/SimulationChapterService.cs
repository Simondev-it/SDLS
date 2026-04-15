using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.SimulationChapter;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SimulationChapterService : ISimulationChapterService
    {
        private readonly ISimulationChapterRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public SimulationChapterService(
            ISimulationChapterRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<SimulationChapterDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetAllAsync(id, name, description, status, role);
            return _mapper.Map<List<SimulationChapterDTO>>(entities);
        }

        public async Task<PagedResult<SimulationChapterDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, name, description, status);
            var total = items.Count;

            return new PagedResult<SimulationChapterDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<(byte[] Content, string FileName, string ContentType)> ExportToExcelAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null)
        {
            var items = await GetAllAsync(id, name, description, status);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("SimulationChapters");

            var headers = new[] { "Id", "Index", "Name", "Description", "Status", "CreateAt", "UpdateAt" };
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
                worksheet.Cell(r, 2).Value = item.Index;
                worksheet.Cell(r, 3).Value = item.Name;
                worksheet.Cell(r, 4).Value = item.Description ?? string.Empty;
                worksheet.Cell(r, 5).Value = item.Status;
                worksheet.Cell(r, 6).Value = item.CreateAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
                worksheet.Cell(r, 7).Value = item.UpdateAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return (
                stream.ToArray(),
                "simulation-chapters.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<SimulationChapterDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<SimulationChapterDTO>(entity);
        }

        public async Task<SimulationChapterDTO> CreateAsync(SimulationChapterCreateDTO dto)
        {
            var now = DateTimeHelper.GetVietnamNow();

            var entity = new SimulationChapter
            {
                Id = Guid.NewGuid(),
                Index = dto.Index,
                Name = dto.Name,
                Description = dto.Description,
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _repository.AddAsync(entity);
            return _mapper.Map<SimulationChapterDTO>(entity);
        }

        public async Task<SimulationChapterDTO> UpdateAsync(Guid id, SimulationChapterUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy SimulationChapter");

            existing.Name = dto.Name;
            existing.Index = dto.Index;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<SimulationChapterDTO>(existing);
        }

        public async Task<SimulationChapterDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            entity.Status = 0;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<SimulationChapterDTO>(entity);
        }

        public async Task<SimulationChapterDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<SimulationChapterDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }
    }
}