using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _context.Exams
                .Where(e => e.Status == 1 && e.IsRandom == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Exam?> GetByIdAsync(Guid id)
        {
            var exam = await _context.Exams
                .Include(e => e.ExamQuestions.Where(eq => eq.Status == 1))
                .ThenInclude(eq => eq.Question)
                .ThenInclude(q => q.Answers.Where(a => a.Status == 1))
                .Where(e => e.Status == 1)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null)
                return null;

            foreach (var eq in exam.ExamQuestions)
            {
                if (eq.Question != null && eq.Question.Status != 1)
                    eq.Question = null;
            }

            return exam;
        }

        public async Task<Exam?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Exams
                .Include(e => e.ExamQuestions)
                .FirstOrDefaultAsync(e => e.Id == id && e.Status == 1);
        }

        public async Task AddAsync(Exam exam)
        {
            await _context.Exams.AddAsync(exam);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Exam exam)
        {
            await _context.SaveChangesAsync();
        }

        // Giữ hành vi cũ: soft delete
        public async Task DeleteAsync(Guid id)
        {
            await DeleteSoftAsync(id);
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id && e.Status == 1);
            if (exam == null)
                return;

            exam.Status = 0;
            exam.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var exam = await _context.Exams
                .Include(e => e.ExamQuestions)
                .Include(e => e.ExamSessions)
                    .ThenInclude(es => es.ExamDetails)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null)
                return;

            var allExamDetails = exam.ExamSessions
                .SelectMany(x => x.ExamDetails)
                .ToList();

            if (allExamDetails.Any())
                _context.ExamDetails.RemoveRange(allExamDetails);

            if (exam.ExamSessions.Any())
                _context.ExamSessions.RemoveRange(exam.ExamSessions);

            if (exam.ExamQuestions.Any())
                _context.ExamQuestions.RemoveRange(exam.ExamQuestions);

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
        }
    }
}
