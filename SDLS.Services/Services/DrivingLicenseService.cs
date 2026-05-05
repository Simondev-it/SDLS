using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.DrivingLicense;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class DrivingLicenseService : IDrivingLicenseService
    {
        private readonly IDrivingLicenseRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public DrivingLicenseService(
            IDrivingLicenseRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<PagedResult<DrivingLicenseDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? vehicleName = null,
            int page = 1,
            int pageSize = 20)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var all = (await _repository.GetAllAsync(id, name, description, vehicleName, status, role))
                .OrderBy(x => x.Name ?? string.Empty)
                .ThenBy(x => x.Id)
                .ToList();
            var total = all.Count;

            var pagedEntities = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedDtos = _mapper.Map<List<DrivingLicenseDTO>>(pagedEntities);

            return new PagedResult<DrivingLicenseDTO>
            {
                Items = pagedDtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<List<DrivingLicenseDTO>> GetAllNoPagingAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? vehicleName = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var all = (await _repository.GetAllAsync(id, name, description, vehicleName, status, role))
                .OrderBy(x => x.Name ?? string.Empty)
                .ThenBy(x => x.Id)
                .ToList();

            return _mapper.Map<List<DrivingLicenseDTO>>(all);
        }

        public async Task<DrivingLicenseDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<DrivingLicenseDTO>(entity);
        }

        public async Task<DrivingLicenseDTO> CreateAsync(DrivingLicenseCreateDTO dto)
        {
            var now = DateTimeHelper.GetVietnamNow();

            var entity = _mapper.Map<DrivingLicense>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Status = 1;
            entity.Binding = dto.Binding;

            if (dto.Vehicles != null)
            {
                foreach (var vehicle in entity.Vehicles)
                {
                    vehicle.DrivingLicenseId = entity.Id;
                    vehicle.CreateAt = now;
                    vehicle.UpdateAt = now;
                    vehicle.Status = 1;
                }
            }

            await _repository.AddAsync(entity);
            return _mapper.Map<DrivingLicenseDTO>(entity);
        }

        public async Task<DrivingLicenseDTO> UpdateAsync(Guid id, DrivingLicenseUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy DrivingLicense");

            var now = DateTimeHelper.GetVietnamNow();

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.Binding = dto.Binding;
            existing.UpdateAt = now;

            if (dto.Vehicles != null)
            {
                var existingVehiclesById = existing.Vehicles.ToDictionary(v => v.Id, v => v);

                foreach (var vehicleDto in dto.Vehicles)
                {
                    if (vehicleDto.DrivingLicenseId != id)
                        throw ApiException.BadRequest($"Vehicle.DrivingLicenseId ({vehicleDto.DrivingLicenseId}) không khớp DrivingLicense Id ({id}).");

                    if (vehicleDto.Id.HasValue)
                    {
                        if (!existingVehiclesById.TryGetValue(vehicleDto.Id.Value, out var vehicle))
                            throw ApiException.NotFound($"Không tìm thấy Vehicle với Id {vehicleDto.Id.Value}");

                        vehicle.Name = vehicleDto.Name;
                        vehicle.Description = vehicleDto.Description;
                        vehicle.Status = vehicleDto.Status ?? vehicle.Status ?? 1;
                        vehicle.UpdateAt = now;
                    }
                    else
                    {
                        var newVehicle = new Vehicle
                        {
                            DrivingLicenseId = id,
                            Name = vehicleDto.Name,
                            Description = vehicleDto.Description,
                            CreateAt = now,
                            UpdateAt = now,
                            Status = vehicleDto.Status ?? 1
                        };

                        existing.Vehicles.Add(newVehicle);
                    }
                }
            }

            await _repository.UpdateAsync(existing);
            return _mapper.Map<DrivingLicenseDTO>(existing);
        }

        public async Task<DrivingLicenseDTO> DeleteSoftAsync(Guid id)
        {
            var entity = await _repository.GetByIdForUpdateAsync(id);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var nextStatus = entity.Status == 0 ? 1 : 0;

            await _repository.DeleteSoftAsync(id);
            entity.Status = nextStatus;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            if (entity.Vehicles != null)
            {
                if (nextStatus == 0)
                {
                    foreach (var vehicle in entity.Vehicles.Where(v => v.Status == 1))
                    {
                        vehicle.Status = 0;
                        vehicle.UpdateAt = entity.UpdateAt;
                    }
                }
                else
                {
                    foreach (var vehicle in entity.Vehicles.Where(v => v.Status == 0))
                    {
                        vehicle.Status = 1;
                        vehicle.UpdateAt = entity.UpdateAt;
                    }
                }
            }
            return _mapper.Map<DrivingLicenseDTO>(entity);
        }

        public async Task<DrivingLicenseDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<DrivingLicenseDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }

    }
}