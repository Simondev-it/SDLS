using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Model.Helpers;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;
using SDLS.Repositories.Helper;

namespace SDLS.Repositories.Repositories
{
    public class ExamSessionRepository : GenericRepository<ExamSession>, IExamSessionRepository
    {
        public async Task<IEnumerable<ExamSession>> GetAllAsync()
        {
            return await _context.ExamSessions
                .Include(es => es.Exam)
                .Include(es => es.ExamDetails)
                    .ThenInclude(ed => ed.Answer)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamSession>> GetAllAsync(
            Guid? examId = null,
            Guid? userId = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<ExamSession> query = isPrivileged
                ? _context.ExamSessions
                    .Include(es => es.Exam)
                    .Include(es => es.ExamDetails)
                        .ThenInclude(ed => ed.Answer)
                : _context.ExamSessions
                    .Include(es => es.Exam)
                    .Include(es => es.ExamDetails.Where(ed => ed.Status != 0))
                        .ThenInclude(ed => ed.Answer);

            if (examId.HasValue)
                query = query.Where(x => x.ExamId == examId.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(x => x.Exam == null || x.Exam.Status != 0);
            }

            var list = await query.AsNoTracking().ToListAsync();

            if (!isPrivileged)
            {
                foreach (var session in list)
                {
                    session.ExamDetails = session.ExamDetails
                        .Where(ed => ed.Answer == null || ed.Answer.Status != 0)
                        .ToList();
                }
            }

            return list;
        }

        public async Task<ExamSession?> GetByIdAsync(Guid id)
        {
            return await _context.ExamSessions
                .Include(es => es.Exam)
                .Include(es => es.ExamDetails)
                    .ThenInclude(ed => ed.Answer)
                .AsNoTracking()
                .FirstOrDefaultAsync(es => es.Id == id);
        }

        public async Task<ExamSession?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<ExamSession> query = isPrivileged
                ? _context.ExamSessions
                    .Include(es => es.Exam)
                    .Include(es => es.ExamDetails)
                        .ThenInclude(ed => ed.Answer)
                : _context.ExamSessions
                    .Include(es => es.Exam)
                    .Include(es => es.ExamDetails.Where(ed => ed.Status != 0))
                        .ThenInclude(ed => ed.Answer);

            query = query.Where(x => x.Id == id)
                         .ApplyRoleFilter(role);

            if (!isPrivileged)
                query = query.Where(x => x.Exam == null || x.Exam.Status != 0);

            var entity = await query.AsNoTracking().FirstOrDefaultAsync();

            if (entity != null && !isPrivileged)
            {
                entity.ExamDetails = entity.ExamDetails
                    .Where(ed => ed.Answer == null || ed.Answer.Status != 0)
                    .ToList();
            }

            return entity;
        }

        public async Task<ExamSession?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.ExamSessions
                .Include(es => es.ExamDetails)
                .FirstOrDefaultAsync(es => es.Id == id);
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

        public async Task DeleteSoftAsync(Guid id)
        {
            var examSession = await _context.ExamSessions
                .FirstOrDefaultAsync(es => es.Id == id && es.Status == 1);

            if (examSession == null)
                return;

            examSession.Status = 0;
            examSession.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var examSession = await _context.ExamSessions
                .Include(es => es.ExamDetails)
                .FirstOrDefaultAsync(es => es.Id == id);

            if (examSession == null)
                return;

            if (examSession.ExamDetails.Any())
                _context.ExamDetails.RemoveRange(examSession.ExamDetails);

            _context.ExamSessions.Remove(examSession);
            await _context.SaveChangesAsync();
        }

    }
}