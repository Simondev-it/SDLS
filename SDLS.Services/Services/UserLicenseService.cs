using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.UserLicense;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class UserLicenseService : IUserLicenseService
    {
        private readonly IUserLicenseRepository _repository;
        private readonly IMapper _mapper;

        public UserLicenseService(IUserLicenseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<UserLicenseDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? drivingLicenseId = null)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable();

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                filtered = filtered.Where(x => x.UserId == userId.Value);

            if (drivingLicenseId.HasValue)
                filtered = filtered.Where(x => x.DrivingLicenseId == drivingLicenseId.Value);

            return _mapper.Map<List<UserLicenseDTO>>(filtered.ToList());
        }

        public async Task<PagedResult<UserLicenseDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? drivingLicenseId = null,
            int page = 1,
            int pageSize = 20)
        {
            var items = await GetAllAsync(id, userId, drivingLicenseId);
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
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<UserLicenseDTO>(entity) : null;
        }

        public async Task<bool> CreateAsync(UserLicenseCreateDTO dto)
        {
            if (dto.UserId == Guid.Empty || dto.DrivingLicenseId == Guid.Empty)
                throw new ArgumentException("UserId và DrivingLicenseId không được rỗng");

            var existing = await _repository.GetByUserAndDrivingLicenseAsync(dto.UserId, dto.DrivingLicenseId);
            if (existing != null && existing.Any())
                throw new InvalidOperationException("UserLicense cho UserId và DrivingLicenseId này đã tồn tại.");

            var entity = _mapper.Map<UserLicense>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreateAt = DateTime.UtcNow.ToLocalTime();
            entity.UpdateAt = DateTime.UtcNow.ToLocalTime();
            entity.Status = 1;

            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, UserLicenseUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            var isChangingKeys = existing.UserId != dto.UserId || existing.DrivingLicenseId != dto.DrivingLicenseId;
            if (isChangingKeys)
            {
                var conflict = await _repository.GetByUserAndDrivingLicenseAsync(dto.UserId, dto.DrivingLicenseId);
                if (conflict != null && conflict.Any(x => x.Id != id))
                    throw new InvalidOperationException("Cặp UserId và DrivingLicenseId mới đã tồn tại ở record khác.");
            }

            existing.UserId = dto.UserId;
            existing.DrivingLicenseId = dto.DrivingLicenseId;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            existing.Status = dto.Status ?? existing.Status;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
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