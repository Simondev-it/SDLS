using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SavedQuestionRepository : GenericRepository<SavedQuestion>, ISavedQuestionRepository
    {
        public async Task<List<SavedQuestion>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<SavedQuestion> query = isPrivileged
                ? _context.SavedQuestions
                    .Include(x => x.Question).ThenInclude(q => q.Answers)
                : _context.SavedQuestions
                    .Include(x => x.Question).ThenInclude(q => q.Answers.Where(a => a.Status != 0));

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (questionId.HasValue)
                query = query.Where(x => x.QuestionId == questionId.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!isPrivileged)
                query = query.Where(x => x.Question == null || x.Question.Status != 0);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<SavedQuestion?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<SavedQuestion> query = isPrivileged
                ? _context.SavedQuestions
                    .Include(x => x.Question).ThenInclude(q => q.Answers)
                : _context.SavedQuestions
                    .Include(x => x.Question).ThenInclude(q => q.Answers.Where(a => a.Status != 0));

            query = query.Where(x => x.Id == id).ApplyRoleFilter(role);

            if (!isPrivileged)
                query = query.Where(x => x.Question == null || x.Question.Status != 0);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<List<SavedQuestion>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId)
        {
            IQueryable<SavedQuestion> query = _context.SavedQuestions;

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (questionId.HasValue)
                query = query.Where(x => x.QuestionId == questionId.Value);

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(SavedQuestion entity)
        {
            await _context.SavedQuestions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SavedQuestion entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.SavedQuestions
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var entity = await _context.SavedQuestions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            _context.SavedQuestions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}