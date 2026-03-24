using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        public async Task<IEnumerable<Report>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? reportCategoryId = null,
            Guid? simulationId = null,
            Guid? forumPostId = null,
            Guid? forumCommentId = null,
            Guid? questionId = null,
            string? title = null,
            string? content = null,
            int? status = null)
        {
            var query = _context.Reports
                .Include(x => x.ReportCategory)
                .Include(x => x.ForumComment)
                .Include(x => x.ForumPost).ThenInclude(x => x.PostImages)
                .Include(x => x.Question)
                .Include(x => x.Simulation)
                .Where(x => x.Status.HasValue && (x.Status == -1 || x.Status == 1 || x.Status == 2))
                .AsNoTracking()
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (reportCategoryId.HasValue)
                query = query.Where(x => x.ReportCategoryId == reportCategoryId.Value);

            if (simulationId.HasValue)
                query = query.Where(x => x.SimulationId == simulationId.Value);

            if (forumPostId.HasValue)
                query = query.Where(x => x.ForumPostId == forumPostId.Value);

            if (forumCommentId.HasValue)
                query = query.Where(x => x.ForumCommentId == forumCommentId.Value);

            if (questionId.HasValue)
                query = query.Where(x => x.QuestionId == questionId.Value);

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

            return await query.ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(Guid id)
        {
            return await _context.Reports
                .Include(x => x.ReportCategory)
                .Include(x => x.ForumComment)
                .Include(x => x.ForumPost).ThenInclude(x => x.PostImages)
                .Include(x => x.Question)
                .Include(x => x.Simulation)
                .Include(x => x.Resolves.Where(r => r.Status == 1))
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id
                    && x.Status.HasValue
                    && (x.Status == -1 || x.Status == 1 || x.Status == 2));
        }

        public async Task<Report?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Reports
                .FirstOrDefaultAsync(x => x.Id == id
                    && x.Status.HasValue
                    && (x.Status == -1 || x.Status == 1 || x.Status == 2));
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

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.Reports
                .FirstOrDefaultAsync(x => x.Id == id
                    && x.Status.HasValue
                    && (x.Status == -1 || x.Status == 1 || x.Status == 2));

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }
    }
}