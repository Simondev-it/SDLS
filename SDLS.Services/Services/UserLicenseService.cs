using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.UserLicense;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class UserLicenseService : IUserLicenseService
    {
        private readonly IUserLicenseRepository _repository;
        private readonly IDrivingLicenseRepository _drivingLicenseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserLicenseService(
            IUserLicenseRepository repository,
            IDrivingLicenseRepository drivingLicenseRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _drivingLicenseRepository = drivingLicenseRepository;
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
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<UserLicenseDTO>(entity);
        }

        public async Task<UserLicenseDTO> CreateAsync(UserLicenseCreateDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            if (dto.DrivingLicenseId == Guid.Empty)
                throw ApiException.BadRequest("DrivingLicenseId không được rỗng");

            var drivingLicense = await _drivingLicenseRepository.GetByIdAsync(dto.DrivingLicenseId);
            if (drivingLicense == null)
                throw ApiException.BadRequest("DrivingLicenseId không tồn tại");

            var existing = await _repository.GetByUserAndDrivingLicenseAsync(currentUserId, dto.DrivingLicenseId);
            if (existing != null && existing.Any())
                throw ApiException.Conflict("UserLicense cho UserId và DrivingLicenseId này đã tồn tại.");

            var entity = _mapper.Map<UserLicense>(dto);
            entity.Id = Guid.NewGuid();
            entity.UserId = currentUserId;
            entity.CreateAt = DateTimeHelper.GetVietnamNow();
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            entity.Status = 1;

            await _repository.AddAsync(entity);
            return _mapper.Map<UserLicenseDTO>(entity);
        }

        public async Task<UserLicenseDTO> UpdateAsync(Guid id, UserLicenseUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy UserLicense");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);

            var isChangingKeys = existing.UserId != currentUserId || existing.DrivingLicenseId != dto.DrivingLicenseId;

            var drivingLicense = await _drivingLicenseRepository.GetByIdAsync(dto.DrivingLicenseId);
            if (drivingLicense == null)
                throw ApiException.BadRequest("DrivingLicenseId không tồn tại");

            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndDrivingLicenseAsync(currentUserId, dto.DrivingLicenseId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw ApiException.Conflict("Cặp UserId và DrivingLicenseId mới đã tồn tại ở record khác.");
            }

            existing.UserId = currentUserId;
            existing.DrivingLicenseId = dto.DrivingLicenseId;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateAsync(existing);
            return _mapper.Map<UserLicenseDTO>(existing);
        }

        public async Task<UserLicenseDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _repository.DeleteSoftAsync(id);
            entity.Status = 0;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<UserLicenseDTO>(entity);
        }

        public async Task<UserLicenseDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<UserLicenseDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }
    }
}