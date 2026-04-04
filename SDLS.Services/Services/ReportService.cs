using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.Constants;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Notification;
using SDLS.Model.DTOs.Report;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;
        private readonly IResolveRepository _resolveRepository;
        private readonly ISimulationScenarioRepository _simulationScenarioRepository;
        private readonly IForumPostRepository _forumPostRepository;
        private readonly IForumCommentRepository _forumCommentRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IReportCategoryRepository _reportCategoryRepository;
        private readonly INotificationService _notificationService;
        private readonly IExecutionStrategyRepository _executionStrategy;
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ReportService(
            IReportRepository repository,
            IResolveRepository resolveRepository,
            ISimulationScenarioRepository simulationScenarioRepository,
            IForumPostRepository forumPostRepository,
            IForumCommentRepository forumCommentRepository,
            IQuestionRepository questionRepository,
            IReportCategoryRepository reportCategoryRepository,
            INotificationService notificationService,
            IExecutionStrategyRepository executionStrategyRepository,
            AppDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _resolveRepository = resolveRepository;
            _simulationScenarioRepository = simulationScenarioRepository;
            _forumPostRepository = forumPostRepository;
            _forumCommentRepository = forumCommentRepository;
            _questionRepository = questionRepository;
            _reportCategoryRepository = reportCategoryRepository;
            _notificationService = notificationService;
            _executionStrategy = executionStrategyRepository;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
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

            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var filtered = await _repository.GetAllAsync(
                id, userId, reportCategoryId, simulationId, forumPostId, forumCommentId, questionId, title, content, status, role);

            var ordered = filtered.OrderByDescending(x => x.CreateAt).ThenByDescending(x => x.Id).ToList();
            var total = ordered.Count;

            var items = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<ReportDTO>(entity);
        }

        public async Task<bool> CreateAsync(ReportCreateDTO dto)
        {
            return await _executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    if (!dto.SimulationId.HasValue && !dto.ForumPostId.HasValue && !dto.ForumCommentId.HasValue && !dto.QuestionId.HasValue)
                        throw ApiException.BadRequest("Phải có ít nhất 1 đối tượng bị report.");

                    var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
                    var now = DateTime.UtcNow.ToLocalTime();

                    var entity = _mapper.Map<Report>(dto);
                    entity.Id = Guid.NewGuid();
                    entity.UserId = currentUserId;
                    entity.CreateAt = now;
                    entity.UpdateAt = now;
                    entity.Status = -1;
                    entity.Image = dto.Image;

                    await _repository.AddAsync(entity);

                    var instructorUserIds = await _dbContext.Users
                        .AsNoTracking()
                        .Where(x => x.RoleId == RoleConst.INSTRUCTOR_ROLE_ID && x.Status != 0)
                        .Select(x => x.Id)
                        .Distinct()
                        .ToListAsync();

                    if (instructorUserIds.Any())
                    {
                        var notification = new NotificationCreateDTO
                        {
                            Title = "Báo cáo mới",
                            Content = $"Có báo cáo mới cần xử lý: '{entity.Title}'.",
                            Status = 2,
                            UserNotifications = instructorUserIds
                                .Select(userId => new UserNotificationCreateDTO { UserId = userId })
                                .ToList()
                        };

                        await _notificationService.CreateAsync(notification);
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<bool> UpdateAsync(Guid id, ReportUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy Report");

            if (!dto.SimulationId.HasValue && !dto.ForumPostId.HasValue && !dto.ForumCommentId.HasValue && !dto.QuestionId.HasValue)
                throw ApiException.BadRequest("Phải có ít nhất 1 đối tượng bị report.");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            existing.SimulationId = dto.SimulationId;
            existing.ForumPostId = dto.ForumPostId;
            existing.ForumCommentId = dto.ForumCommentId;
            existing.QuestionId = dto.QuestionId;
            existing.ReportCategoryId = dto.ReportCategoryId;
            existing.UserId = currentUserId;
            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.Image = dto.Image;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> ApproveAsync(Guid id, ReportResolveActionDTO dto)
        {
            var report = await _repository.GetByIdForUpdateAsync(id);
            if (report == null)
                throw ApiException.NotFound("Không tìm thấy Report");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTime.UtcNow.ToLocalTime();

            report.Status = 1;
            report.UpdateAt = now;

            await _repository.UpdateAsync(report);

            var resolve = new Resolve
            {
                Id = Guid.NewGuid(),
                ReportId = id,
                UserId = currentUserId,
                Title = dto.Title.Trim(),
                Content = dto.Content.Trim(),
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _resolveRepository.AddAsync(resolve);
            return true;
        }

        public async Task<bool> DisapproveAsync(Guid id, ReportResolveActionDTO dto)
        {
            var report = await _repository.GetByIdForUpdateAsync(id);
            if (report == null)
                throw ApiException.NotFound("Không tìm thấy Report");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTime.UtcNow.ToLocalTime();

            report.Status = 3;
            report.UpdateAt = now;

            await _repository.UpdateAsync(report);

            var resolve = new Resolve
            {
                Id = Guid.NewGuid(),
                ReportId = id,
                UserId = currentUserId,
                Title = dto.Title.Trim(),
                Content = dto.Content.Trim(),
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _resolveRepository.AddAsync(resolve);
            return true;
        }

        public async Task<bool> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var report = await _repository.GetByIdAsync(id, role);
            if (report == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            return true;
        }

        public async Task<bool> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var report = await _repository.GetByIdAsync(id, role);
            if (report == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteHardAsync(id);
            return true;
        }
    }
}