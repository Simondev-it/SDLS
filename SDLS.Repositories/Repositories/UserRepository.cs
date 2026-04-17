using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Repositories
{

    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(x => x.Role)
                .Include(x => x.LearningProgresses)
                    .ThenInclude(lp => lp.Question)
                .Include(x => x.ExamSessions)
                    .ThenInclude(es => es.Exam)
                .Include(x => x.UserLicenses)
                    .ThenInclude(ul => ul.DrivingLicense)
                .Include(x => x.LessonProgresses)
                    .ThenInclude(lp => lp.QuestionLesson)
                .Include(x => x.SimulationSessions)
                    .ThenInclude(ss => ss.SituationExam)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(x => x.Role)
                .Include(x => x.LearningProgresses)
                    .ThenInclude(lp => lp.Question)
                .Include(x => x.ExamSessions)
                    .ThenInclude(es => es.Exam)
                .Include(x => x.UserLicenses)
                    .ThenInclude(ul => ul.DrivingLicense)
                .Include(x => x.LessonProgresses)
                    .ThenInclude(lp => lp.QuestionLesson)
                .Include(x => x.SimulationSessions)
                    .ThenInclude(ss => ss.SituationExam)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        // ✅ IMPLEMENT CREATE
        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(x => x.LearningProgresses)
                    .ThenInclude(lp => lp.Question)
                .Include(x => x.ExamSessions)
                    .ThenInclude(es => es.Exam)
                .Include(x => x.UserLicenses)
                    .ThenInclude(ul => ul.DrivingLicense)
                .Include(x => x.LessonProgresses)
                    .ThenInclude(lp => lp.QuestionLesson)
                .Include(x => x.SimulationSessions)
                    .ThenInclude(ss => ss.SituationExam)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllBasicAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            user.Id = Guid.NewGuid();
            user.CreateAt = DateTime.UtcNow;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            user.UpdateAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveAsync(User user)
        {
            _context.Users.Remove(user);
        }
    }
}
