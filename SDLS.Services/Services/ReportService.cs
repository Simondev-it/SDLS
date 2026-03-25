using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Report;
using SDLS.Model.Enumerations;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public ReportService(IReportRepository repository, IStorageService storageService, IMapper mapper)
        {
            _repository = repository;
            _storageService = storageService;
            _mapper = mapper;
        }

        public async Task<PagedResult<ReportDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? reportCategoryId = null,
            Guid? simulationId = null,
            Guid? forumPostId = null,
            Guid? forumCommentId = null,
            Guid? questionId = null,
            string? title = null,
            string? content = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var filtered = await _repository.GetAllAsync(
                id, userId, reportCategoryId, simulationId, forumPostId, forumCommentId, questionId, title, content, status);

            var ordered = filtered.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Id).ToList();
            var total = ordered.Count;

            var items = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<ReportDTO>
            {
                Items = _mapper.Map<List<ReportDTO>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<ReportDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<ReportDTO>(entity);
        }

        public async Task<bool> CreateAsync(ReportCreateDTO dto)
        {
            if (!dto.SimulationId.HasValue && !dto.ForumPostId.HasValue && !dto.ForumCommentId.HasValue && !dto.QuestionId.HasValue)
                throw new ArgumentException("Phải có ít nhất 1 đối tượng bị report.");

            var now = DateTime.UtcNow.ToLocalTime();

            var entity = _mapper.Map<Report>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Status = -1;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                entity.Image = await _storageService.UploadImageAsync(dto.ImageFile, ImageTarget.ReportImage, entity.Id);
            }

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, ReportUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy Report");

            if (!dto.SimulationId.HasValue && !dto.ForumPostId.HasValue && !dto.ForumCommentId.HasValue && !dto.QuestionId.HasValue)
                throw new ArgumentException("Phải có ít nhất 1 đối tượng bị report.");

            existing.SimulationId = dto.SimulationId;
            existing.ForumPostId = dto.ForumPostId;
            existing.ForumCommentId = dto.ForumCommentId;
            existing.QuestionId = dto.QuestionId;
            existing.ReportCategoryId = dto.ReportCategoryId;
            existing.UserId = dto.UserId;
            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                existing.Image = await _storageService.UploadImageAsync(dto.ImageFile, ImageTarget.ReportImage, id);
            }

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
    }
}