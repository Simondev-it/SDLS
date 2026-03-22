using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class QuestionCategoryRepository : GenericRepository<QuestionCategory>, IQuestionCategoryRepository
    {
        public async Task<List<QuestionCategory>> GetAllAsync()
        {
            return await _context.QuestionCategories.Where(qc => qc.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<QuestionCategory?> GetByIdAsync(Guid id)
        {
            return await _context.QuestionCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<QuestionCategory?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.QuestionCategories
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(QuestionCategory entity)
        {
            await _context.QuestionCategories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(QuestionCategory entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.QuestionCategories.FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
            if (existing == null) return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _context.SaveChangesAsync();
        }
    }
}