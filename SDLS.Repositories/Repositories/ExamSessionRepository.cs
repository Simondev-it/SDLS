using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class ExamSessionRepository : GenericRepository<ExamSession>, IExamSessionRepository
    {
        public async Task<IEnumerable<ExamSession>> GetAllAsync()
        {
            return await _context.ExamSessions
                .Include(es => es.Exam)
                .Include(es => es.ExamDetails.Where(ed => ed.Status == 1))
                    .ThenInclude(ed => ed.Answer)
                .Where(es => es.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ExamSession?> GetByIdAsync(Guid id)
        {
            return await _context.ExamSessions
                .Include(es => es.Exam)
                .Include(es => es.ExamDetails.Where(ed => ed.Status == 1))
                    .ThenInclude(ed => ed.Answer)
                .Where(es => es.Status == 1)
                .AsNoTracking()
                .FirstOrDefaultAsync(es => es.Id == id);
        }

        public async Task<ExamSession?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.ExamSessions
                .Include(es => es.ExamDetails)
                .FirstOrDefaultAsync(es => es.Id == id && es.Status == 1);
        }

        public async Task AddAsync(ExamSession examSession)
        {
            await _context.ExamSessions.AddAsync(examSession);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExamSession examSession)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var examSession = await _context.ExamSessions
                .FirstOrDefaultAsync(es => es.Id == id && es.Status == 1);

            if (examSession == null)
                return;

            examSession.Status = 0;
            examSession.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _context.SaveChangesAsync();
        }
    }
}