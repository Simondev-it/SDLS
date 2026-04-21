using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.Constants;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ForumPost;
using SDLS.Model.DTOs.Report;
using SDLS.Model.DTOs.SystemConfig;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class SystemConfigService : ISystemConfigService
    {
        private readonly ISystemConfigRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public SystemConfigService(
            ISystemConfigRepository repository,
            IHttpContextAccessor httpContextAccessor,
            AppDbContext dbContext,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<SystemConfigDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            int? value = null,
            string? description = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetAllAsync(id, name, value, description, status, role);
            return _mapper.Map<List<SystemConfigDTO>>(entities);
        }

        public async Task<PagedResult<SystemConfigDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            int? value = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, name, value, description, status);
            var total = items.Count;

            return new PagedResult<SystemConfigDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<SystemDashboardSummaryDTO> GetDashboardSummaryAsync()
        {
            var now = DateTimeHelper.GetVietnamNow();
            var targetYear = now.Year;
            var targetMonth = now.Month;

            var monthStart = new DateTime(targetYear, targetMonth, 1);
            var monthEnd = monthStart.AddMonths(1);

            var todayStart = now.Date;
            var yesterdayStart = todayStart.AddDays(-1);

            var weekStart = todayStart.AddDays(-6);
            var weekEnd = todayStart.AddDays(1);

            const string contentChangeCategoryIdRaw = "46936a12-e8cc-4298-beca-d381f74ed50e";
            var contentChangeCategoryId = Guid.Parse(contentChangeCategoryIdRaw);

            var totalPaymentAmount = await _dbContext.Payments
                .Where(x => x.Status == 1 && x.CreateAt.HasValue && x.CreateAt.Value >= monthStart && x.CreateAt.Value < monthEnd)
                .SumAsync(x => (long?)x.Amount) ?? 0L;

            var roleIds = new[]
            {
                RoleConst.USER_ROLE_ID,
                RoleConst.INSTRUCTOR_ROLE_ID,
                RoleConst.GUEST_ROLE_ID
            };

            var roleLabels = new Dictionary<Guid, string>
            {
                [RoleConst.USER_ROLE_ID] = "Student",
                [RoleConst.INSTRUCTOR_ROLE_ID] = "Instructor",
                [RoleConst.GUEST_ROLE_ID] = "Guest"
            };

            var userRoleStats = await _dbContext.Users
                .Where(x => roleIds.Contains(x.RoleId))
                .GroupBy(x => x.RoleId)
                .Select(g => new
                {
                    RoleId = g.Key,
                    TotalUsers = g.Count(),
                    ActiveUsers = g.Count(x => x.Status == 1)
                })
                .ToListAsync();

            var roleStatDtos = roleIds
                .Select(roleId =>
                {
                    var roleStat = userRoleStats.FirstOrDefault(x => x.RoleId == roleId);
                    var totalUsers = roleStat?.TotalUsers ?? 0;
                    var activeUsers = roleStat?.ActiveUsers ?? 0;

                    return new RoleUserStatsDTO
                    {
                        RoleId = roleId,
                        RoleName = roleLabels[roleId],
                        TotalUsers = totalUsers,
                        ActiveUsers = activeUsers,
                        ActiveRate = CalculateRate(totalUsers, activeUsers)
                    };
                })
                .ToList();

            var monthlyExam = await GetSessionStatsAsync(
                _dbContext.ExamSessions.AsNoTracking(), monthStart, monthEnd);
            var monthlySimulation = await GetSessionStatsAsync(
                _dbContext.SimulationSessions.AsNoTracking(), monthStart, monthEnd);

            var weeklyExam = await GetSessionStatsAsync(
                _dbContext.ExamSessions.AsNoTracking(), weekStart, weekEnd);
            var weeklySimulation = await GetSessionStatsAsync(
                _dbContext.SimulationSessions.AsNoTracking(), weekStart, weekEnd);

            var pendingReviewReportsCount = await _dbContext.Reports
                .CountAsync(x => x.Status == -1 && x.ReportCategoryId != contentChangeCategoryId);

            var pendingReviewReportsToday = await _dbContext.Reports
                .CountAsync(x => x.Status == -1 && x.ReportCategoryId != contentChangeCategoryId
                    && x.CreateAt.HasValue && x.CreateAt.Value >= todayStart && x.CreateAt.Value < todayStart.AddDays(1));

            var pendingReviewReportsYesterday = await _dbContext.Reports
                .CountAsync(x => x.Status == -1 && x.ReportCategoryId != contentChangeCategoryId
                    && x.CreateAt.HasValue && x.CreateAt.Value >= yesterdayStart && x.CreateAt.Value < todayStart);

            var pendingForumPostsCount = await _dbContext.ForumPosts
                .CountAsync(x => x.Status == -1);

            var pendingContentChangeReportsCount = await _dbContext.Reports
                .CountAsync(x => x.Status == -1 && x.ReportCategoryId == contentChangeCategoryId);

            var pendingContentChangeReportsToday = await _dbContext.Reports
                .CountAsync(x => x.Status == -1 && x.ReportCategoryId == contentChangeCategoryId
                    && x.CreateAt.HasValue && x.CreateAt.Value >= todayStart && x.CreateAt.Value < todayStart.AddDays(1));

            var pendingContentChangeReportsYesterday = await _dbContext.Reports
                .CountAsync(x => x.Status == -1 && x.ReportCategoryId == contentChangeCategoryId
                    && x.CreateAt.HasValue && x.CreateAt.Value >= yesterdayStart && x.CreateAt.Value < todayStart);

            return new SystemDashboardSummaryDTO
            {
                Year = targetYear,
                Month = targetMonth,
                TotalPaymentAmount = totalPaymentAmount,

                MonthlyExamSessionCount = monthlyExam.TotalCount,
                MonthlySimulationSessionCount = monthlySimulation.TotalCount,

                WeeklyExamStats = weeklyExam,
                WeeklySimulationStats = weeklySimulation,
                MonthlyExamStats = monthlyExam,
                MonthlySimulationStats = monthlySimulation,

                UserRoleStats = roleStatDtos,

                PendingReviewReportsCount = pendingReviewReportsCount,
                PendingReviewReportsIncreaseFromYesterday = pendingReviewReportsToday - pendingReviewReportsYesterday,

                PendingForumPostsCount = pendingForumPostsCount,

                PendingContentChangeReportsCount = pendingContentChangeReportsCount,
                PendingContentChangeReportsIncreaseFromYesterday = pendingContentChangeReportsToday - pendingContentChangeReportsYesterday
            };
        }

        public async Task<InstructorDashboardSummaryDTO> GetInstructorDashboardSummaryAsync()
        {
            var contentChangeCategoryId = Guid.Parse("46936a12-e8cc-4298-beca-d381f74ed50e");

            var forumPostCount = await _dbContext.ForumPosts
                .AsNoTracking()
                .CountAsync();

            var forumPosts = await _dbContext.ForumPosts
                .AsNoTracking()
                .Include(x => x.User)
                    .ThenInclude(u => u.Role)
                .Include(x => x.PostImages.Where(img => img.Status != 0))
                .OrderByDescending(x => x.CreateAt ?? DateTime.MinValue)
                .ThenByDescending(x => x.Id)
                .Take(4)
                .ToListAsync();

            var contentChangeRequestCount = await _dbContext.Reports
                .AsNoTracking()
                .CountAsync(x => x.ReportCategoryId == contentChangeCategoryId && x.Status == -1);

            var contentChangeReports = await _dbContext.Reports
                .AsNoTracking()
                .Include(x => x.User)
                    .ThenInclude(u => u.Role)
                .Include(x => x.ReportCategory)
                .Include(x => x.ForumPost)
                .Include(x => x.Question)
                .Include(x => x.Simulation)
                .Where(x => x.ReportCategoryId == contentChangeCategoryId && x.Status == -1)
                .OrderByDescending(x => x.CreateAt ?? DateTime.MinValue)
                .ThenByDescending(x => x.Id)
                .Take(4)
                .ToListAsync();

            var contentIssueReportCount = await _dbContext.Reports
                .AsNoTracking()
                .CountAsync(x => (x.SimulationId != null || x.QuestionId != null) && x.Status == -1);

            var communityReportCount = await _dbContext.Reports
                .AsNoTracking()
                .CountAsync(x => (x.ForumPostId != null || x.ForumCommentId != null) && x.Status == -1);

            return new InstructorDashboardSummaryDTO
            {
                ForumPostCount = forumPostCount,
                ForumPosts = _mapper.Map<List<ForumPostDTO>>(forumPosts),
                ContentChangeRequestCount = contentChangeRequestCount,
                ContentChangeRequests = _mapper.Map<List<ReportDTO>>(contentChangeReports),
                ContentIssueReportCount = contentIssueReportCount,
                CommunityReportCount = communityReportCount
            };
        }

        public async Task<(byte[] Content, string FileName, string ContentType)> ExportDashboardSummaryExcelAsync()
        {
            var summary = await GetDashboardSummaryAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("DashboardSummary");

            var row = 1;
            worksheet.Cell(row, 1).Value = "Metric";
            worksheet.Cell(row, 2).Value = "Value";
            worksheet.Range(row, 1, row, 2).Style.Font.Bold = true;
            row++;

            void AddMetric(string metric, object? value)
            {
                worksheet.Cell(row, 1).Value = metric;
                worksheet.Cell(row, 2).Value = value?.ToString() ?? string.Empty;
                row++;
            }

            AddMetric("Year", summary.Year);
            AddMetric("Month", summary.Month);
            AddMetric("TotalPaymentAmount", summary.TotalPaymentAmount);
            AddMetric("MonthlyExamSessionCount", summary.MonthlyExamSessionCount);
            AddMetric("MonthlySimulationSessionCount", summary.MonthlySimulationSessionCount);

            AddMetric("WeeklyExam_TotalCount", summary.WeeklyExamStats.TotalCount);
            AddMetric("WeeklyExam_PassRate", summary.WeeklyExamStats.PassRate);
            AddMetric("WeeklyExam_FailRate", summary.WeeklyExamStats.FailRate);

            AddMetric("WeeklySimulation_TotalCount", summary.WeeklySimulationStats.TotalCount);
            AddMetric("WeeklySimulation_PassRate", summary.WeeklySimulationStats.PassRate);
            AddMetric("WeeklySimulation_FailRate", summary.WeeklySimulationStats.FailRate);

            AddMetric("MonthlyExam_TotalCount", summary.MonthlyExamStats.TotalCount);
            AddMetric("MonthlyExam_PassRate", summary.MonthlyExamStats.PassRate);
            AddMetric("MonthlyExam_FailRate", summary.MonthlyExamStats.FailRate);

            AddMetric("MonthlySimulation_TotalCount", summary.MonthlySimulationStats.TotalCount);
            AddMetric("MonthlySimulation_PassRate", summary.MonthlySimulationStats.PassRate);
            AddMetric("MonthlySimulation_FailRate", summary.MonthlySimulationStats.FailRate);

            AddMetric("PendingReviewReportsCount", summary.PendingReviewReportsCount);
            AddMetric("PendingReviewReportsIncreaseFromYesterday", summary.PendingReviewReportsIncreaseFromYesterday);
            AddMetric("PendingForumPostsCount", summary.PendingForumPostsCount);
            AddMetric("PendingContentChangeReportsCount", summary.PendingContentChangeReportsCount);
            AddMetric("PendingContentChangeReportsIncreaseFromYesterday", summary.PendingContentChangeReportsIncreaseFromYesterday);

            row += 1;
            worksheet.Cell(row, 1).Value = "RoleName";
            worksheet.Cell(row, 2).Value = "TotalUsers";
            worksheet.Cell(row, 3).Value = "ActiveUsers";
            worksheet.Cell(row, 4).Value = "ActiveRate";
            worksheet.Range(row, 1, row, 4).Style.Font.Bold = true;
            row++;

            foreach (var roleStat in summary.UserRoleStats)
            {
                worksheet.Cell(row, 1).Value = roleStat.RoleName;
                worksheet.Cell(row, 2).Value = roleStat.TotalUsers;
                worksheet.Cell(row, 3).Value = roleStat.ActiveUsers;
                worksheet.Cell(row, 4).Value = roleStat.ActiveRate;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return (
                stream.ToArray(),
                $"dashboard-summary-{DateTimeHelper.GetVietnamNow():yyyyMMdd-HHmmss}.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<SystemConfigDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<SystemConfigDTO>(entity);
        }

        public async Task<SystemConfigDTO> CreateAsync(SystemConfigCreateDTO dto)
        {
            var now = DateTimeHelper.GetVietnamNow();

            var entity = new SystemConfig
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Value = dto.Value,
                Description = dto.Description,
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _repository.AddAsync(entity);
            return _mapper.Map<SystemConfigDTO>(entity);
        }

        public async Task<SystemConfigDTO> UpdateAsync(Guid id, SystemConfigUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm th?y SystemConfig");

            existing.Name = dto.Name;
            existing.Value = dto.Value;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<SystemConfigDTO>(existing);
        }

        public async Task<SystemConfigDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            entity.Status = 0;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<SystemConfigDTO>(entity);
        }

        public async Task<SystemConfigDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<SystemConfigDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }

        private static async Task<SessionStatsDTO> GetSessionStatsAsync<T>(
            IQueryable<T> query,
            DateTime start,
            DateTime end)
            where T : class
        {
            var grouped = await query
                .Where(x => EF.Property<int?>(x, "Status") == 1
                            && EF.Property<DateTime?>(x, "CreateAt").HasValue
                            && EF.Property<DateTime?>(x, "CreateAt").Value >= start
                            && EF.Property<DateTime?>(x, "CreateAt").Value < end)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Passed = g.Count(x => EF.Property<bool>(x, "IsPassed"))
                })
                .FirstOrDefaultAsync();

            var total = grouped?.Total ?? 0;
            var passed = grouped?.Passed ?? 0;
            var failed = total - passed;

            return new SessionStatsDTO
            {
                TotalCount = total,
                PassRate = CalculateRate(total, passed),
                FailRate = CalculateRate(total, failed)
            };
        }

        private static double CalculateRate(int total, int value)
        {
            if (total <= 0) return 0;
            return Math.Round(value * 100d / total, 2, MidpointRounding.AwayFromZero);
        }
    }
}
