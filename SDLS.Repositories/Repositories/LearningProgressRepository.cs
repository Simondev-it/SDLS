using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class LearningProgressRepository : GenericRepository<LearningProgress>, ILearningProgressRepository
    {
        public async Task<LearningProgress?> GetByIdAsync(Guid id, string? role = null)
        {
            var query = _context.LearningProgresses
                .Include(lp => lp.Question)
                .Include(lp => lp.User)
                .Where(lp => lp.Id == id);

            query = query.ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
            {
                query = query.Where(lp =>
                    (lp.Question == null || lp.Question.Status != 0) &&
                    (lp.User == null || lp.User.Status != 0));
            }

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<List<LearningProgress>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null,
            string? role = null)
        {
            var query = _context.LearningProgresses
                .Include(lp => lp.Question)
                .Include(lp => lp.User)
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(lp => lp.Id == id.Value);

            if (userId.HasValue)
                query = query.Where(lp => lp.UserId == userId.Value);

            if (questionId.HasValue)
                query = query.Where(lp => lp.QuestionId == questionId.Value);

            if (status.HasValue)
                query = query.Where(lp => lp.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
            {
                query = query.Where(lp =>
                    (lp.Question == null || lp.Question.Status != 0) &&
                    (lp.User == null || lp.User.Status != 0));
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(LearningProgress entity)
        {
            await _context.LearningProgresses.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LearningProgress entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.LearningProgresses
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.LearningProgresses
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            _context.LearningProgresses.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LearningProgress>> GetByUserAndQuestionAsync(
            Guid? userId,
            Guid? questionId,
            int? status = null,
            string? role = null)
        {
            var query = _context.LearningProgresses
                .Include(lp => lp.Question)
                .Include(lp => lp.User)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(lp => lp.UserId == userId.Value);

            if (questionId.HasValue)
                query = query.Where(lp => lp.QuestionId == questionId.Value);

            if (status.HasValue)
                query = query.Where(lp => lp.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
            {
                query = query.Where(lp =>
                    (lp.Question == null || lp.Question.Status != 0) &&
                    (lp.User == null || lp.User.Status != 0));
            }

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
