using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SavedQuestionRepository : GenericRepository<SavedQuestion>, ISavedQuestionRepository
    {
        public async Task<List<SavedQuestion>> GetAllAsync()
        {
            return await _context.SavedQuestions
                .Include(x => x.Question).ThenInclude(q => q.Answers)
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SavedQuestion?> GetByIdAsync(Guid id)
        {
            return await _context.SavedQuestions
                .Include(x => x.Question).ThenInclude(q => q.Answers)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<List<SavedQuestion>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId)
        {
            var query = _context.SavedQuestions
                .Where(x => x.Status == 1);

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

        public async Task DeleteAsync(Guid id)
        {
            await DeleteHardAsync(id);
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.SavedQuestions
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
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