using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.QuestionChapter;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;
using System.Globalization;
using System.Text;

namespace SDLS.Services.Services
{
    public class QuestionChapterService : IQuestionChapterService
    {
        private readonly IQuestionChapterRepository _repository;
        private readonly IDrivingLicenseRepository _drivingLicenseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public QuestionChapterService(
            IQuestionChapterRepository repository,
            IDrivingLicenseRepository drivingLicenseRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _drivingLicenseRepository = drivingLicenseRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<PagedResult<QuestionChapterDTO>> GetAllAsync(
            Guid? id = null,
            Guid? drivingLicenseId = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var filtered = await _repository.GetAllAsync(
                id, drivingLicenseId, name, description, status, role);

            var total = filtered.Count();

            var items = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<QuestionChapterDTO>
            {
                Items = _mapper.Map<List<QuestionChapterDTO>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<QuestionChapterDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<QuestionChapterDTO>(entity);
        }

        public async Task<QuestionChapterDTO> CreateAsync(QuestionChapterCreateDTO dto)
        {
            var drivingLicense = await _drivingLicenseRepository.GetByIdAsync(dto.DrivingLicenseId);
            if (drivingLicense == null)
                throw ApiException.BadRequest($"DrivingLicense with ID {dto.DrivingLicenseId} does not exist");

            var now = DateTimeHelper.GetVietnamNow();

            var entity = _mapper.Map<QuestionChapter>(dto);
            entity.Id = Guid.NewGuid();
            entity.Index = dto.Index;
            entity.CreateAt = now;
            entity.UpdateAt = now;
            entity.Status = 1;

            await _repository.AddAsync(entity);
            return _mapper.Map<QuestionChapterDTO>(entity);
        }

        public async Task<QuestionChapterDTO> UpdateAsync(Guid id, QuestionChapterUpdateDTO dto)
        {
            var existing = await _repository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy QuestionChapter");

            var drivingLicense = await _drivingLicenseRepository.GetByIdAsync(dto.DrivingLicenseId);
            if (drivingLicense == null)
                throw ApiException.BadRequest($"DrivingLicense with ID {dto.DrivingLicenseId} does not exist");

            existing.DrivingLicenseId = dto.DrivingLicenseId;
            existing.Index = dto.Index;
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status ?? existing.Status ?? 1;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();

            await _repository.UpdateAsync(existing);
            return _mapper.Map<QuestionChapterDTO>(existing);
        }

        public async Task<QuestionChapterDTO> DeleteSoftAsync(Guid id)
        {
            var entity = await _repository.GetByIdForUpdateAsync(id);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var nextStatus = entity.Status == 0 ? 1 : 0;

            await _repository.DeleteSoftAsync(id);
            entity.Status = nextStatus;
            entity.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<QuestionChapterDTO>(entity);
        }

        public async Task<QuestionChapterDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var entity = await _repository.GetByIdAsync(id, role);
            if (entity == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<QuestionChapterDTO>(entity);
            await _repository.DeleteHardAsync(id);
            return result;
        }
    }
}