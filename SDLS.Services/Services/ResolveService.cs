using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Resolve;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class ResolveService : IResolveService
    {
        private readonly IResolveRepository _repository;
        private readonly IExecutionStrategyRepository _executionStrategyRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;

        public ResolveService(
            IResolveRepository repository,
            IExecutionStrategyRepository executionStrategyRepository,
            IReportRepository reportRepository,
            IMapper mapper)
        {
            _repository = repository;
            _executionStrategyRepository = executionStrategyRepository;
            _reportRepository = reportRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ResolveDTO>> GetAllAsync(
            Guid? id = null,
            Guid? reportId = null,
            Guid? userId = null,
            string? title = null,
            string? content = null,
            int? status = 1,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var all = await _repository.GetAllAsync(id, reportId, userId, title, content, status);
            var ordered = all.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Id).ToList();
            var total = ordered.Count;

            var items = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<ResolveDTO>
            {
                Items = _mapper.Map<List<ResolveDTO>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<ResolveDTO?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<ResolveDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(ResolveCreateDTO dto)
        {
            if (dto.ReportId == Guid.Empty || dto.UserId == Guid.Empty)
                throw new ArgumentException("ReportId và UserId không được rỗng.");

            await _executionStrategyRepository.ExecuteAsync(async () =>
            {
                await using var transaction = await _repository.BeginTransactionAsync();
                try
                {
                    var now = DateTime.UtcNow.ToLocalTime();

                    var report = await _reportRepository.GetByIdAsync(dto.ReportId);

                    if (report == null)
                        throw new KeyNotFoundException("Không tìm thấy Report.");

                    var entity = new Resolve
                    {
                        Id = Guid.NewGuid(),
                        ReportId = dto.ReportId,
                        UserId = dto.UserId,
                        Title = dto.Title,
                        Content = dto.Content,
                        CreateAt = now,
                        UpdateAt = now,
                        Status = 1
                    };

                    await _repository.AddAsync(entity);

                    report.Status = 1;
                    report.UpdateAt = now;
                    await _reportRepository.UpdateAsync(report);

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, ResolveUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy Resolve");

            existing.ReportId = dto.ReportId;
            existing.UserId = dto.UserId;
            existing.Title = dto.Title;
            existing.Content = dto.Content;
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