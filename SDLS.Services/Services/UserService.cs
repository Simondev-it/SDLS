using AutoMapper;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.User;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDLS.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionLessonRepository _questionLessonRepository;
        private readonly IUserLicenseRepository _userLicenseRepository;
        private readonly IDrivingLicenseRepository _drivingLicenseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediaImageService _mediaImageService;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IQuestionRepository questionRepository,
            IQuestionLessonRepository questionLessonRepository,
            IUserLicenseRepository userLicenseRepository,
            IDrivingLicenseRepository drivingLicenseRepository,
            IHttpContextAccessor httpContextAccessor,
            IMediaImageService mediaImageService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _questionRepository = questionRepository;
            _questionLessonRepository = questionLessonRepository;
            _userLicenseRepository = userLicenseRepository;
            _drivingLicenseRepository = drivingLicenseRepository;
            _httpContextAccessor = httpContextAccessor;
            _mediaImageService = mediaImageService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDTO>> GetAllAsync(
            Guid? id = null,
            Guid? roleId = null,
            string? email = null,
            string? name = null,
            int? status = null)
        {
            var users = await _userRepository.GetAllBasicAsync();

            users = ApplyFilters(users, id, roleId, email, name, status);

            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<PagedResult<UserDTO>> GetAllWithPagingAsync(
            Guid? id = null,
            Guid? roleId = null,
            string? email = null,
            string? name = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var users = await _userRepository.GetAllBasicAsync();

            users = ApplyFilters(users, id, roleId, email, name, status);
            users = users
                .OrderByDescending(x => x.CreateAt ?? DateTime.MinValue)
                .ThenByDescending(x => x.Id);

            var mappedUsers = _mapper.Map<List<UserDTO>>(users);

            var totalCount = mappedUsers.Count;

            return new PagedResult<UserDTO>
            {
                Items = mappedUsers.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        private static IEnumerable<User> ApplyFilters(
            IEnumerable<User> users,
            Guid? id,
            Guid? roleId,
            string? email,
            string? name,
            int? status)
        {
            var query = users.AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (roleId.HasValue)
                query = query.Where(x => x.RoleId == roleId.Value);

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.Contains(email, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            return query;
        }

        public async Task<UserDTO?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            var totalQuestionCount = (await _questionRepository.GetAllAsync(status: 1)).Count();
            var totalQuestionLessonCount = (await _questionLessonRepository.GetAllAsync(status: 1)).Count();
            var dto = _mapper.Map<UserDTO>(user);
            PopulateUserGetData(dto, user, totalQuestionCount, totalQuestionLessonCount);
            return dto;
        }

        public async Task<UserDTO?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            var totalQuestionCount = (await _questionRepository.GetAllAsync(status: 1)).Count();
            var totalQuestionLessonCount = (await _questionLessonRepository.GetAllAsync(status: 1)).Count();
            var dto = _mapper.Map<UserDTO>(user);
            PopulateUserGetData(dto, user, totalQuestionCount, totalQuestionLessonCount);
            return dto;
        }

        public async Task<UserExamStatisticsDTO> GetCurrentUserStatisticsAsync()
        {
            // 1. Lấy ID người dùng hiện tại từ JWT
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            // 2. Lấy dữ liệu user kèm các Session thi
            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null) throw ApiException.NotFound("Người dùng không tồn tại.");

            // 3. Tính toán thống kê Lý thuyết (ExamSessions)
            var activeExamSessions = user.ExamSessions.Where(x => x.Status == 1).ToList();
            var theoryStats = new ExamStats
            {
                TotalAttempts = activeExamSessions.Count,
                PassedCount = activeExamSessions.Count(x => x.IsPassed),
                FailedCount = activeExamSessions.Count(x => !x.IsPassed),
                PassRate = CalculatePassRate(activeExamSessions.Count, activeExamSessions.Count(x => x.IsPassed))
            };
            theoryStats.FailRate = theoryStats.TotalAttempts > 0 ? Math.Round(100 - theoryStats.PassRate, 2) : 0;

            // 4. Tính toán thống kê Mô phỏng (SimulationSessions)
            var activeSimSessions = user.SimulationSessions.Where(x => x.Status == 1).ToList();
            var simStats = new ExamStats
            {
                TotalAttempts = activeSimSessions.Count,
                PassedCount = activeSimSessions.Count(x => x.IsPassed),
                FailedCount = activeSimSessions.Count(x => !x.IsPassed),
                PassRate = CalculatePassRate(activeSimSessions.Count, activeSimSessions.Count(x => x.IsPassed))
            };
            simStats.FailRate = simStats.TotalAttempts > 0 ? Math.Round(100 - simStats.PassRate, 2) : 0;

            return new UserExamStatisticsDTO
            {
                TheoryStats = theoryStats,
                SimulationStats = simStats
            };
        }

        public async Task<UserDTO> CreateAsync(UserCreateDTO user)
        {
            var existingByEmail = await _userRepository.GetByEmailAsync(user.Email);
            if (existingByEmail != null)
                throw ApiException.Conflict("Email đã tồn tại.");

            var entity = _mapper.Map<User>(user);
            entity.Status = user.Status ?? 1;

            await _userRepository.AddAsync(entity);
            return _mapper.Map<UserDTO>(entity);
        }

        public async Task<UserDTO?> UpdateAsync(Guid id, UserUpdateDTO user)
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null) return null;
            var oldAvatar = existing.Avatar;

            if (!string.IsNullOrWhiteSpace(user.Email) &&
                !string.Equals(existing.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingByEmail = await _userRepository.GetByEmailAsync(user.Email);
                if (existingByEmail != null && existingByEmail.Id != id)
                    throw ApiException.Conflict("Email đã tồn tại.");
            }

            existing.Name = user.Name;
            existing.Email = user.Email;
            existing.RoleId = user.RoleId;
            existing.Avatar = user.Avatar;
            existing.Phone = user.Phone;
            existing.Gender = user.Gender;
            existing.Description = user.Description;
            existing.DateOfBirth = user.DateOfBirth;
            existing.LicenseType = user.LicenseType;
            existing.Status = user.Status ?? existing.Status;

            await UpdateUserLicensesAsync(existing.Id, user.DrivingLicenseIds);

            if (!string.Equals(oldAvatar, user.Avatar, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(oldAvatar))
                await _mediaImageService.DeleteAsync(oldAvatar, "UserAvatar");

            await _userRepository.UpdateAsync(existing);
            return _mapper.Map<UserDTO>(existing);
        }

        public async Task<bool> ChangePasswordCurrentUserAsync(UserChangePasswordDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var existing = await _userRepository.GetByIdAsync(currentUserId);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy user hiện tại.");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, existing.Password))
                throw ApiException.BadRequest("Mật khẩu hiện tại không đúng.");

            existing.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userRepository.UpdateAsync(existing);

            return true;
        }

        public async Task<UserDTO?> ToggleActiveStatusAsync(Guid id)
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null) return null;

            if (existing.Status != 0 && existing.Status != 1)
                throw ApiException.BadRequest("Chỉ hỗ trợ chuyển trạng thái giữa 0 và 1.");

            existing.Status = existing.Status == 1 ? 0 : 1;
            await _userRepository.UpdateAsync(existing);
            return _mapper.Map<UserDTO>(existing);
        }

        public async Task<UserDTO?> ToggleLockStatusAsync(Guid id)
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Status = existing.Status == 2 ? 1 : 2;
            await _userRepository.UpdateAsync(existing);
            return _mapper.Map<UserDTO>(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(user.Avatar))
                await _mediaImageService.DeleteAsync(user.Avatar, "UserAvatar");

            // Nếu bạn có soft delete thì sửa lại
            // user.IsDeleted = true;

            // Hard delete:
            await _userRepository.RemoveAsync(user);
            await _userRepository.SaveAsync();

            return true;
        }

        private async Task UpdateUserLicensesAsync(Guid userId, List<Guid>? drivingLicenseIds)
        {
            var existingUserLicenses = await _userLicenseRepository.GetByUserAndDrivingLicenseAsync(userId, null);

            foreach (var existingUserLicense in existingUserLicenses)
                await _userLicenseRepository.DeleteHardAsync(existingUserLicense.Id);

            if (drivingLicenseIds == null)
                return;

            var distinctIds = drivingLicenseIds
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();

            foreach (var drivingLicenseId in distinctIds)
            {
                var drivingLicense = await _drivingLicenseRepository.GetByIdAsync(drivingLicenseId);
                if (drivingLicense == null)
                    throw ApiException.BadRequest($"DrivingLicenseId không tồn tại: {drivingLicenseId}");
            }

            foreach (var drivingLicenseId in distinctIds)
            {
                await _userLicenseRepository.AddAsync(new UserLicense
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DrivingLicenseId = drivingLicenseId,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow,
                    Status = 1
                });
            }
        }

        private static void PopulateUserGetData(UserDTO dto, User user, int totalQuestionCount, int totalQuestionLessonCount)
        {
            dto.TotalQuestionCount = totalQuestionCount;
            dto.TotalQuestionLessonCount = totalQuestionLessonCount;

            dto.LearningProgresses = dto.LearningProgresses
                .Where(x => x.Status != 0)
                .ToList();
            dto.ExamSessions = dto.ExamSessions
                .Where(x => x.Status != 0)
                .ToList();
            dto.LessonProgresses = dto.LessonProgresses
                .Where(x => x.Status != 0)
                .ToList();
            dto.SimulationSessions = dto.SimulationSessions
                .Where(x => x.Status != 0)
                .ToList();
            dto.UserLicenses = dto.UserLicenses
                .Where(x => x.Status != 0)
                .ToList();

            dto.LearningProgressQuestionCount = user.LearningProgresses
                .Where(x => x.Status == 1)
                .Select(x => x.QuestionId)
                .Distinct()
                .Count();

            dto.ExamPassRate = CalculatePassRate(
                user.ExamSessions.Count(x => x.Status == 1),
                user.ExamSessions.Count(x => x.Status == 1 && x.IsPassed));

            dto.SimulationPassRate = CalculatePassRate(
                user.SimulationSessions.Count(x => x.Status == 1),
                user.SimulationSessions.Count(x => x.Status == 1 && x.IsPassed));
        }

        private static double CalculatePassRate(int totalCount, int passedCount)
        {
            if (totalCount <= 0)
                return 0;

            var rate = passedCount * 100d / totalCount;
            return Math.Round(rate, 2, MidpointRounding.AwayFromZero);
        }
    }
}
