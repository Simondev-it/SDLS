using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Model.Helpers;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        public async Task<IEnumerable<Exam>> GetAllAsync(
            Guid? userId = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<Exam> query = isPrivileged
                ? _context.Exams
                    .Include(e => e.ExamQuestions)
                : _context.Exams
                    .Include(e => e.ExamQuestions.Where(eq => eq.Status != 0));

            query = query.Where(e => e.IsRandom == false);

            if (userId.HasValue)
                query = query.Where(e => e.UserId == userId.Value);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Exam?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<Exam> query = isPrivileged
                ? _context.Exams
                    .Include(e => e.ExamQuestions)
                        .ThenInclude(eq => eq.Question)
                            .ThenInclude(q => q.Answers)
                : _context.Exams
                    .Include(e => e.ExamQuestions.Where(eq => eq.Status != 0))
                        .ThenInclude(eq => eq.Question)
                            .ThenInclude(q => q.Answers.Where(a => a.Status != 0));

            query = query.Where(e => e.Id == id)
                         .ApplyRoleFilter(role);

            var exam = await query.AsNoTracking().FirstOrDefaultAsync();
            if (exam == null)
                return null;

            if (!isPrivileged)
            {
                exam.ExamQuestions = exam.ExamQuestions
                    .Where(eq => eq.Question == null || eq.Question.Status != 0)
                    .ToList();
            }

            return exam;
        }

        public async Task<Exam?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Exams
                .Include(e => e.ExamQuestions)
                .FirstOrDefaultAsync(e => e.Id == id);
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

        public async Task DeleteSoftAsync(Guid id)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id && e.Status == 1);
            if (exam == null) return;

            exam.Status = 0;
            exam.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var exam = await _context.Exams
                .Include(e => e.ExamQuestions)
                .Include(e => e.ExamSessions)
                    .ThenInclude(es => es.ExamDetails)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null) return;

            var allExamDetails = exam.ExamSessions.SelectMany(x => x.ExamDetails).ToList();

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
