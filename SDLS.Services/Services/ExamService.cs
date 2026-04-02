using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Exam;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDLS.Services.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ExamService(
            IExamRepository examRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _examRepository = examRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<PagedResult<ExamDTO>> GetAllAsync(
            Guid? userId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var allExams = await _examRepository.GetAllAsync(userId, status, role);

            var total = allExams.Count();

            var pagedEntities = allExams
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedDtos = _mapper.Map<List<ExamDTO>>(pagedEntities);

            return new PagedResult<ExamDTO>
            {
                Items = pagedDtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<ExamDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var exam = await _examRepository.GetByIdAsync(id, role);
            if (exam == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<ExamDTO>(exam);
        }

        public async Task<bool> CreateAsync(ExamCreateDTO dto)
        {
            if (dto.ExamQuestions == null || !dto.ExamQuestions.Any())
                throw new ArgumentException("Exam must have at least 1 exam question");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTime.UtcNow.ToLocalTime();

            var newExam = _mapper.Map<Exam>(dto);
            newExam.Id = Guid.NewGuid();
            newExam.UserId = currentUserId;
            newExam.CreateAt = now;
            newExam.UpdateAt = now;
            newExam.Status = 1;

            foreach (var examQuestion in newExam.ExamQuestions)
            {
                examQuestion.ExamId = newExam.Id;
                examQuestion.CreateAt = now;
                examQuestion.UpdateAt = now;
                examQuestion.Status = 1;
            }

            await _examRepository.AddAsync(newExam);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, ExamUpdateDTO dto)
        {
            var existing = await _examRepository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy exam");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTime.UtcNow.ToLocalTime();

            existing.UserId = currentUserId;
            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.Duration = dto.Duration;
            existing.PassScore = dto.PassScore;
            existing.IsRandom = dto.IsRandom;
            existing.UpdateAt = now;

            if (dto.ExamQuestions != null)
            {
                var existingExamQuestionsById = existing.ExamQuestions.ToDictionary(eq => eq.Id, eq => eq);

                foreach (var examQuestionDto in dto.ExamQuestions)
                {
                    if (examQuestionDto.ExamId != id)
                        throw new ArgumentException($"ExamQuestion.ExamId ({examQuestionDto.ExamId}) không khớp Exam Id ({id}).");

                    if (examQuestionDto.Id.HasValue)
                    {
                        if (!existingExamQuestionsById.TryGetValue(examQuestionDto.Id.Value, out var examQuestion))
                            throw new KeyNotFoundException($"Không tìm thấy ExamQuestion với Id {examQuestionDto.Id.Value}");

                        examQuestion.QuestionId = examQuestionDto.QuestionId;
                        examQuestion.UpdateAt = now;
                        examQuestion.Status = examQuestionDto.Status ?? examQuestion.Status ?? 1;
                    }
                    else
                    {
                        var newExamQuestion = new ExamQuestion
                        {
                            ExamId = id,
                            QuestionId = examQuestionDto.QuestionId,
                            CreateAt = now,
                            UpdateAt = now,
                            Status = examQuestionDto.Status ?? 1
                        };

                        existing.ExamQuestions.Add(newExamQuestion);
                    }
                }
            }

            await _examRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteSoftAsync(Guid id)
        {
            await _examRepository.DeleteSoftAsync(id);
            return true;
        }

        public async Task<bool> DeleteHardAsync(Guid id)
        {
            await _examRepository.DeleteHardAsync(id);
            return true;
        }
    }
}
