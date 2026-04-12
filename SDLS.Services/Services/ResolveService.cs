using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.Notification;
using SDLS.Model.DTOs.Resolve;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class ResolveService : IResolveService
    {
        private readonly IResolveRepository _repository;
        private readonly IExecutionStrategyRepository _executionStrategyRepository;
        private readonly IReportRepository _reportRepository;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ResolveService(
            IResolveRepository repository,
            IExecutionStrategyRepository executionStrategyRepository,
            IReportRepository reportRepository,
            INotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _executionStrategyRepository = executionStrategyRepository;
            _reportRepository = reportRepository;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<PagedResult<ResolveDTO>> GetAllAsync(
            Guid? id = null,
            Guid? reportId = null,
            Guid? userId = null,
            string? title = null,
            string? content = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var all = await _repository.GetAllAsync(id, reportId, userId, title, content, status, role);
            var ordered = all.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Id).ToList();
            var total = ordered.Count;

            var items = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<ResolveDTO>(entity);
        }

        public async Task<ResolveDTO> CreateAsync(ResolveCreateDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.ReportId == Guid.Empty)
                throw ApiException.BadRequest("ReportId không được rỗng.");

            return await _executionStrategyRepository.ExecuteAsync(async () =>
            {
                await using var transaction = await _repository.BeginTransactionAsync();
                try
                {
                    var now = DateTimeHelper.GetVietnamNow();

                    var report = await _reportRepository.GetByIdAsync(dto.ReportId);
                    if (report == null)
                        throw ApiException.NotFound("Không tìm thấy Report.");

                    var entity = new Resolve
                    {
                        Id = Guid.NewGuid(),
                        ReportId = dto.ReportId,
                        UserId = currentUserId,
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

                    var notificationDto = new NotificationCreateDTO
                    {
                        Title = "Báo cáo đã được xử lý",
                        Content = "Báo cáo '" + report.Title + "' của bạn đã được xử lý.",
                        Status = 2,
                        UserNotifications = new List<UserNotificationCreateDTO>
                        {
                            new UserNotificationCreateDTO
                            {
                                UserId = report.UserId
                            }
                        }
                    };

                    await _notificationService.CreateAsync(notificationDto);

                    await transaction.CommitAsync();
                    return _mapper.Map<ResolveDTO>(entity);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<ResolveDTO> UpdateAsync(Guid id, ResolveUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy Resolve");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            existing.ReportId = dto.ReportId;
            existing.UserId = currentUserId;
            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<ResolveDTO>(existing);
        }

        public async Task<ResolveDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var existing = await _repository.GetByIdAsync(id, role);
            if (existing == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<ResolveDTO>(existing);
        }

        public async Task<ResolveDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var existing = await _repository.GetByIdAsync(id, role);
            if (existing == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<ResolveDTO>(existing);
            await _repository.DeleteHardAsync(id);
            return result;
        }
    }
}