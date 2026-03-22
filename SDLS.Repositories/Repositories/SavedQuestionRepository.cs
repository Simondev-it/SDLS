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
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SavedQuestion?> GetByIdAsync(Guid id)
        {
            return await _context.SavedQuestions
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<List<SavedQuestion>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId)
        {
            var query = _context.SavedQuestions.Where(x => x.Status == 1);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (questionId.HasValue)
                query = query.Where(x => x.QuestionId == questionId.Value);

            return await query.ToListAsync();
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
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.SavedQuestions.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}