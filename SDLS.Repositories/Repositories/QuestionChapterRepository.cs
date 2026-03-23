using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class QuestionChapterRepository : GenericRepository<QuestionChapter>, IQuestionChapterRepository
    {
        public async Task<IEnumerable<QuestionChapter>> GetAllAsync()
        {
            return await _context.QuestionChapters
                .Include(x => x.DrivingLicense)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<QuestionChapter?> GetByIdAsync(Guid id)
        {
            return await _context.QuestionChapters
                .Include(x => x.DrivingLicense)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<QuestionChapter?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.QuestionChapters
                .Include(x => x.DrivingLicense)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(QuestionChapter chapter)
        {
            await _context.QuestionChapters.AddAsync(chapter);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(QuestionChapter chapter)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var chapter = await _context.QuestionChapters
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (chapter == null)
                return;

            chapter.Status = 0;
            chapter.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }
    }
}