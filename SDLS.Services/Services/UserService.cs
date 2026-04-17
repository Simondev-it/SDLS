using SDLS.Model.Models;
using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.User;
using SDLS.Model.Models;
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
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IQuestionRepository questionRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _questionRepository = questionRepository;
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
            var dto = _mapper.Map<UserDTO>(user);
            PopulateUserGetData(dto, user, totalQuestionCount);
            return dto;
        }

        public async Task<UserDTO?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            var totalQuestionCount = (await _questionRepository.GetAllAsync(status: 1)).Count();
            var dto = _mapper.Map<UserDTO>(user);
            PopulateUserGetData(dto, user, totalQuestionCount);
            return dto;
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

            if (!string.IsNullOrWhiteSpace(user.Password))
                existing.Password = user.Password;

            await _userRepository.UpdateAsync(existing);
            return _mapper.Map<UserDTO>(existing);
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

            if (existing.Status != 1 && existing.Status != 2)
                throw ApiException.BadRequest("Chỉ hỗ trợ chuyển trạng thái giữa 1 và 2.");

            existing.Status = existing.Status == 1 ? 2 : 1;
            await _userRepository.UpdateAsync(existing);
            return _mapper.Map<UserDTO>(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            // Nếu bạn có soft delete thì sửa lại
            // user.IsDeleted = true;

            // Hard delete:
            await _userRepository.RemoveAsync(user);
            await _userRepository.SaveAsync();

            return true;
        }

        private static void PopulateUserGetData(UserDTO dto, User user, int totalQuestionCount)
        {
            dto.TotalQuestionCount = totalQuestionCount;

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
