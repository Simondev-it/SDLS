using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.UserLicense;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class UserLicenseService : IUserLicenseService
    {
        private readonly IUserLicenseRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserLicenseService(
            IUserLicenseRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<UserLicenseDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? drivingLicenseId = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entities = await _repository.GetAllAsync(id, userId, drivingLicenseId, status, role);
            return _mapper.Map<List<UserLicenseDTO>>(entities);
        }

        public async Task<PagedResult<UserLicenseDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? drivingLicenseId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, drivingLicenseId, status);
            var total = items.Count;

            return new PagedResult<UserLicenseDTO>
            {
                Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<UserLicenseDTO?> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            return entity != null ? _mapper.Map<UserLicenseDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(UserLicenseCreateDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.DrivingLicenseId == Guid.Empty)
                throw new ArgumentException("DrivingLicenseId không được rỗng");

            var existing = await _repository.GetByUserAndDrivingLicenseAsync(currentUserId, dto.DrivingLicenseId);
            if (existing != null && existing.Any())
                throw new InvalidOperationException("UserLicense cho UserId và DrivingLicenseId này đã tồn tại.");

            var entity = _mapper.Map<UserLicense>(dto);
            entity.Id = Guid.NewGuid();
            entity.UserId = currentUserId;
            entity.CreateAt = DateTime.UtcNow.ToLocalTime();
            entity.UpdateAt = DateTime.UtcNow.ToLocalTime();
            entity.Status = 1;

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, UserLicenseUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null) return false;

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            var isChangingKeys = existing.UserId != currentUserId || existing.DrivingLicenseId != dto.DrivingLicenseId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndDrivingLicenseAsync(currentUserId, dto.DrivingLicenseId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw new InvalidOperationException("Cặp UserId và DrivingLicenseId mới đã tồn tại ở record khác.");
            }

            existing.UserId = currentUserId;
            existing.DrivingLicenseId = dto.DrivingLicenseId;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteSoftAsync(Guid id)
        {
            await _repository.DeleteSoftAsync(id);
            return true;
        }

        public async Task<bool> DeleteHardAsync(Guid id)
        {
            await _repository.DeleteHardAsync(id);
            return true;
        }
    }
}