using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.DrivingLicense;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class DrivingLicenseService : IDrivingLicenseService
    {
        private readonly IDrivingLicenseRepository _repository;
        private readonly IMapper _mapper;

        public DrivingLicenseService(IDrivingLicenseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<DrivingLicenseDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = 1,
            string? vehicleName = null,
            int page = 1,
            int pageSize = 20)
        {
            var all = await _repository.GetAllAsync();
            var filtered = all.AsEnumerable();

            if (id.HasValue)
                filtered = filtered.Where(x => x.Id == id.Value);

            if (!string.IsNullOrWhiteSpace(name))
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.Name)
                    && x.Name.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(description))
                filtered = filtered.Where(x => !string.IsNullOrWhiteSpace(x.Description)
                    && x.Description.Contains(description.Trim(), StringComparison.OrdinalIgnoreCase));

            if (status.HasValue)
                filtered = filtered.Where(x => x.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(vehicleName))
            {
                var keyword = vehicleName.Trim();
                filtered = filtered.Where(x =>
                    x.Vehicles != null &&
                    x.Vehicles.Any(v => v.Status == 1 &&
                        !string.IsNullOrWhiteSpace(v.Name) &&
                        v.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            }

            var total = filtered.Count();

            var pagedEntities = filtered
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

        public async Task<DrivingLicenseDTO> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<DrivingLicenseDTO>(entity);
        }

        public async Task<bool> CreateAsync(DrivingLicenseCreateDTO dto)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var entity = _mapper.Map<DrivingLicense>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Status = 1;

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
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, DrivingLicenseUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy DrivingLicense");

            var now = DateTime.UtcNow.ToLocalTime();

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = now;

            if (dto.Vehicles != null)
            {
                var existingVehiclesById = existing.Vehicles.ToDictionary(v => v.Id, v => v);

                foreach (var vehicleDto in dto.Vehicles)
                {
                    if (vehicleDto.DrivingLicenseId != id)
                        throw new ArgumentException($"Vehicle.DrivingLicenseId ({vehicleDto.DrivingLicenseId}) không khớp DrivingLicense Id ({id}).");

                    if (vehicleDto.Id.HasValue)
                    {
                        if (!existingVehiclesById.TryGetValue(vehicleDto.Id.Value, out var vehicle))
                            throw new KeyNotFoundException($"Không tìm thấy Vehicle với Id {vehicleDto.Id.Value}");

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
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}