using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionChapter;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using System.Globalization;
using System.Text;

namespace SDLS.Services.Services
{
    public class QuestionChapterService : IQuestionChapterService
    {
        private readonly IQuestionChapterRepository _repository;
        private readonly IMapper _mapper;

        public QuestionChapterService(IQuestionChapterRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<QuestionChapterDTO>> GetAllAsync(
            Guid? id = null,
            Guid? drivingLicenseId = null,
            string? name = null,
            string? description = null,
            int? status = 1,
            int page = 1,
            int pageSize = 20)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable();

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (drivingLicenseId.HasValue)
                filtered = filtered.Where(x => x.DrivingLicenseId == drivingLicenseId.Value);

            if (!string.IsNullOrWhiteSpace(name))
                filtered = filtered.Where(x => ContainsNormalized(x.Name, name));

            if (!string.IsNullOrWhiteSpace(description))
                filtered = filtered.Where(x => ContainsNormalized(x.Description, description));

            if (status.HasValue)
                filtered = filtered.Where(x => x.Status == status.Value);

            var total = filtered.Count();

            var items = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<QuestionChapterDTO>
            {
                Items = _mapper.Map<List<QuestionChapterDTO>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        private static bool ContainsNormalized(string? source, string? keyword)
        {
            var left = NormalizeText(source);
            var right = NormalizeText(keyword);

            if (string.IsNullOrWhiteSpace(right))
                return true;

            return left.Contains(right, StringComparison.Ordinal);
        }

        private static string NormalizeText(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var formD = input.Trim().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in formD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            var normalized = sb.ToString().Normalize(NormalizationForm.FormC)
                .Replace('đ', 'd')
                .Replace('Đ', 'D');

            normalized = string.Join(' ', normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            return normalized.ToLowerInvariant();
        }

        public async Task<QuestionChapterDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<QuestionChapterDTO>(entity);
        }

        public async Task<bool> CreateAsync(QuestionChapterCreateDTO dto)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var entity = _mapper.Map<QuestionChapter>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Status = 1;

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, QuestionChapterUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy QuestionChapter");

            existing.DrivingLicenseId = dto.DrivingLicenseId;
            existing.Name = dto.Name;
            existing.Description = dto.Description;
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