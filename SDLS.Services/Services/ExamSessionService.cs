using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.ExamSession;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
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
        private readonly IMapper _mapper;

        public ExamSessionService(IExamSessionRepository examSessionRepository, IMapper mapper)
        {
            _examSessionRepository = examSessionRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ExamSessionDTO>> GetAllAsync(
            Guid? examId = null,
            Guid? userId = null,
            int page = 1,
            int pageSize = 20)
        {
            var allSessions = await _examSessionRepository.GetAllAsync();
            var filtered = allSessions.AsEnumerable();

            if (examId.HasValue)
                filtered = filtered.Where(x => x.ExamId == examId.Value);

            if (userId.HasValue)
                filtered = filtered.Where(x => x.UserId == userId.Value);

            var total = filtered.Count();

            var pagedEntities = filtered
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
            var examSession = await _examSessionRepository.GetByIdAsync(id);
            if (examSession == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<ExamSessionDTO>(examSession);
        }

        public async Task<bool> CreateAsync(ExamSessionCreateDTO dto)
        {
            if (dto.ExamDetails == null || !dto.ExamDetails.Any())
                throw new ArgumentException("ExamSession must have at least 1 exam detail");

            var now = DateTime.UtcNow.ToLocalTime();

            var newExamSession = _mapper.Map<ExamSession>(dto);
            newExamSession.Id = Guid.NewGuid();
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
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, ExamSessionUpdateDTO dto)
        {
            var existing = await _examSessionRepository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy exam session");

            var now = DateTime.UtcNow.ToLocalTime();

            existing.ExamId = dto.ExamId;
            existing.UserId = dto.UserId;
            existing.Score = dto.Score;
            existing.IsPassed = dto.IsPassed;
            existing.UpdateAt = now;

            if (dto.ExamDetails != null)
            {
                var existingDetailsById = existing.ExamDetails.ToDictionary(ed => ed.Id, ed => ed);

                foreach (var detailDto in dto.ExamDetails)
                {
                    if (detailDto.ExamSessionId != id)
                        throw new ArgumentException($"ExamDetail.ExamSessionId ({detailDto.ExamSessionId}) không khớp ExamSession Id ({id}).");

                    if (detailDto.Id.HasValue)
                    {
                        if (!existingDetailsById.TryGetValue(detailDto.Id.Value, out var detail))
                            throw new KeyNotFoundException($"Không tìm thấy ExamDetail với Id {detailDto.Id.Value}");

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
            return true;
        }

        public async Task<bool> DeleteSoftAsync(Guid id)
        {
            await _examSessionRepository.DeleteSoftAsync(id);
            return true;
        }

        public async Task<bool> DeleteHardAsync(Guid id)
        {
            await _examSessionRepository.DeleteHardAsync(id);
            return true;
        }
    }
}