using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        public async Task<IEnumerable<Report>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            List<Guid>? reportCategoryIds = null,
            string? roleName = null,
            Guid? simulationId = null,
            Guid? forumPostId = null,
            Guid? forumCommentId = null,
            Guid? questionId = null,
            bool? hasSimulation = null,
            bool? hasForumPost = null,
            bool? hasForumComment = null,
            bool? hasQuestion = null,
            string? title = null,
            string? content = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<Report> query = isPrivileged
                    ? _context.Reports
                        .Include(x => x.ReportCategory)
                        .Include(x => x.ForumComment)
                        .Include(x => x.ForumPost).ThenInclude(x => x.PostImages)
                        .Include(x => x.Question)
                        .Include(x => x.Simulation)
                        .Include(x => x.User).ThenInclude(x => x.Role)
                        .Include(x => x.Resolves)
                    : _context.Reports
                    .Include(x => x.ReportCategory)
                    .Include(x => x.ForumComment)
                    .Include(x => x.ForumPost).ThenInclude(x => isPrivileged ? x.PostImages : x.PostImages.Where(pi => pi.Status != 0))
                    .Include(x => x.Question)
                    .Include(x => x.Simulation)
                    .Include(x => x.User).ThenInclude(x => x.Role)
                    .Include(x => x.Resolves);

            if (reportCategoryIds != null && reportCategoryIds.Any())
            {
                query = query.Where(x => reportCategoryIds.Contains(x.ReportCategoryId));
            }

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                query = query.Where(x => x.User != null && x.User.Role != null && x.User.Role.Name == roleName);
            }

            if (id.HasValue) query = query.Where(x => x.Id == id.Value);
            if (userId.HasValue) query = query.Where(x => x.UserId == userId.Value);
            if (simulationId.HasValue) query = query.Where(x => x.SimulationId == simulationId.Value);
            if (forumPostId.HasValue) query = query.Where(x => x.ForumPostId == forumPostId.Value);
            if (forumCommentId.HasValue) query = query.Where(x => x.ForumCommentId == forumCommentId.Value);
            if (questionId.HasValue) query = query.Where(x => x.QuestionId == questionId.Value);

            if (hasSimulation == true || hasForumPost == true || hasForumComment == true || hasQuestion == true)
            {
                query = query.Where(x => 
                    (hasSimulation == true && x.SimulationId != null) ||
                    (hasForumPost == true && x.ForumPostId != null) ||
                    (hasForumComment == true && x.ForumCommentId != null) ||
                    (hasQuestion == true && x.QuestionId != null)
                );
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                var keyword = title.Trim();
                query = query.Where(x => x.Title != null && EF.Functions.ILike(x.Title, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                var keyword = content.Trim();
                query = query.Where(x => x.Content != null && EF.Functions.ILike(x.Content, $"%{keyword}%"));
            }

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(x =>
                    (x.ReportCategory == null || x.ReportCategory.Status != 0) &&
                    (x.ForumComment == null || x.ForumComment.Status != 0) &&
                    (x.ForumPost == null || x.ForumPost.Status != 0) &&
                    (x.Question == null || x.Question.Status != 0) &&
                    (x.Simulation == null || x.Simulation.Status != 0));
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<Report> query = isPrivileged
                ? _context.Reports
                    .Include(x => x.ReportCategory)
                    .Include(x => x.ForumComment)
                    .Include(x => x.ForumPost).ThenInclude(x => x.PostImages)
                    .Include(x => x.Question)
                    .Include(x => x.Simulation)
                    .Include(x => x.User).ThenInclude(x => x.Role)
                    .Include(x => x.Resolves)
                : _context.Reports
                    .Include(x => x.ReportCategory)
                    .Include(x => x.ForumComment)
                    .Include(x => x.ForumPost).ThenInclude(x => x.PostImages.Where(pi => pi.Status != 0))
                    .Include(x => x.Question)
                    .Include(x => x.Simulation)
                    .Include(x => x.User).ThenInclude(x => x.Role)
                    .Include(x => x.Resolves.Where(r => r.Status != 0));

            query = query.Where(x => x.Id == id)
                         .ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(x =>
                    (x.ReportCategory == null || x.ReportCategory.Status != 0) &&
                    (x.ForumComment == null || x.ForumComment.Status != 0) &&
                    (x.ForumPost == null || x.ForumPost.Status != 0) &&
                    (x.Question == null || x.Question.Status != 0) &&
                    (x.Simulation == null || x.Simulation.Status != 0));
            }

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<Report?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Reports
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Report entity)
        {
            await _context.Reports.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Report entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.Reports
                .FirstOrDefaultAsync(x => x.Id == id
                    && x.Status.HasValue
                    && (x.Status == -1 || x.Status == 1 || x.Status == 2));

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.Reports
                .Include(x => x.Resolves)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            if (existing.Resolves.Any())
                _context.Resolves.RemoveRange(existing.Resolves);

            _context.Reports.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}