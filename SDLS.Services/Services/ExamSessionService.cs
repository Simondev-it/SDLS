using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ExamSession;
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
    public class ExamSessionService : IExamSessionService
    {
        private readonly IExamSessionRepository _examSessionRepository;
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExamSessionService(IExamSessionRepository examSessionRepository, IExamRepository examRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _examSessionRepository = examSessionRepository;
            _examRepository = examRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<ExamSessionDTO>> GetAllAsync(
            Guid? examId = null,
            Guid? userId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var allSessions = await _examSessionRepository.GetAllAsync(examId, userId, status, role);

            var total = allSessions.Count();

            var pagedEntities = allSessions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedDtos = _mapper.Map<List<ExamSessionDTO>>(pagedEntities);

            return new PagedResult<ExamSessionDTO>
            {
                Items = pagedDtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<ExamSessionDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var examSession = await _examSessionRepository.GetByIdAsync(id, role);

            if (examSession == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<ExamSessionDTO>(examSession);
        }

        public async Task<ExamSessionDTO> CreateAsync(ExamSessionCreateDTO dto)
        {
            var exam = await _examRepository.GetByIdAsync(dto.ExamId);
            if (exam == null)
                throw ApiException.BadRequest($"Exam with ID {dto.ExamId} does not exist.");

            if (dto.ExamDetails == null || !dto.ExamDetails.Any())
                throw ApiException.BadRequest("ExamSession must have at least 1 exam detail");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTime.UtcNow.ToLocalTime();

            var newExamSession = _mapper.Map<ExamSession>(dto);
            newExamSession.Id = Guid.NewGuid();
            newExamSession.UserId = currentUserId;
            newExamSession.TotalDuration = dto.TotalDuration;
            newExamSession.CreateAt = now;
            newExamSession.UpdateAt = now;
            newExamSession.Status = 1;

            foreach (var detail in newExamSession.ExamDetails)
            {
                detail.ExamSessionId = newExamSession.Id;
                detail.CreateAt = now;
                detail.UpdateAt = now;
                detail.Status = 1;
            }

            await _examSessionRepository.AddAsync(newExamSession);
            return _mapper.Map<ExamSessionDTO>(newExamSession);
        }

        public async Task<ExamSessionDTO> UpdateAsync(Guid id, ExamSessionUpdateDTO dto)
        {
            var existing = await _examSessionRepository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy exam session");

            var exam = await _examRepository.GetByIdAsync(dto.ExamId);
            if (exam == null)
                throw ApiException.BadRequest($"Exam with ID {dto.ExamId} does not exist.");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTime.UtcNow.ToLocalTime();

            existing.ExamId = dto.ExamId;
            existing.UserId = currentUserId;
            existing.Score = dto.Score;
            existing.TotalDuration = dto.TotalDuration;
            existing.IsPassed = dto.IsPassed;
            existing.UpdateAt = now;

            if (dto.ExamDetails != null)
            {
                var existingDetailsById = existing.ExamDetails.ToDictionary(ed => ed.Id, ed => ed);

                foreach (var detailDto in dto.ExamDetails)
                {
                    if (detailDto.ExamSessionId != id)
                        throw ApiException.BadRequest($"ExamDetail.ExamSessionId ({detailDto.ExamSessionId}) không khớp ExamSession Id ({id}).");

                    if (detailDto.Id.HasValue)
                    {
                        if (!existingDetailsById.TryGetValue(detailDto.Id.Value, out var detail))
                            throw ApiException.NotFound($"Không tìm thấy ExamDetail với Id {detailDto.Id.Value}");

                        detail.AnswerId = detailDto.AnswerId;
                        detail.UpdateAt = now;
                        detail.Status = detailDto.Status ?? detail.Status ?? 1;
                    }
                    else
                    {
                        var newDetail = new ExamDetail
                        {
                            ExamSessionId = id,
                            AnswerId = detailDto.AnswerId,
                            CreateAt = now,
                            UpdateAt = now,
                            Status = detailDto.Status ?? 1
                        };

                        existing.ExamDetails.Add(newDetail);
                    }
                }
            }

            await _examSessionRepository.UpdateAsync(existing);
            return _mapper.Map<ExamSessionDTO>(existing);
        }

        public async Task<ExamSessionDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var examSession = await _examSessionRepository.GetByIdAsync(id, role);

            if (examSession == null)
                throw ApiException.NotFound($"Not found with ID {id}");
            await _examSessionRepository.DeleteSoftAsync(id);
            examSession.Status = 0;
            examSession.UpdateAt = DateTime.UtcNow.ToLocalTime();
            return _mapper.Map<ExamSessionDTO>(examSession);
        }

        public async Task<ExamSessionDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var examSession = await _examSessionRepository.GetByIdAsync(id, role);

            if (examSession == null)
                throw ApiException.NotFound($"Not found with ID {id}");
            var result = _mapper.Map<ExamSessionDTO>(examSession);
            await _examSessionRepository.DeleteHardAsync(id);
            return result;
        }
    }
}