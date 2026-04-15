using AutoMapper;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Exam;
using SDLS.Model.Helpers;
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
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ExamService(
            IExamRepository examRepository,
            IQuestionRepository questionRepository,
            ITagRepository tagRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _examRepository = examRepository;
            _questionRepository = questionRepository;
            _tagRepository = tagRepository;
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
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<ExamDTO>(exam);
        }

        public async Task<ExamDTO> CreateAsync(ExamCreateDTO dto)
        {
            if (dto.ExamQuestions == null || !dto.ExamQuestions.Any())
                throw ApiException.BadRequest("Exam must have at least 1 exam question");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTimeHelper.GetVietnamNow();

            var newExam = _mapper.Map<Exam>(dto);
            newExam.Id = Guid.NewGuid();
            newExam.UserId = currentUserId;
            newExam.CreateAt = now;
            newExam.UpdateAt = now;
            newExam.Status = 1;

            foreach (var examQuestion in newExam.ExamQuestions)
            {
                var question = await _questionRepository.GetByIdAsync(examQuestion.QuestionId);

                if (question == null)
                    throw ApiException.NotFound("Question không tồn tại");

                examQuestion.ExamId = newExam.Id;
                examQuestion.CreateAt = now;
                examQuestion.UpdateAt = now;
                examQuestion.Status = 1;
            }

            await _examRepository.AddAsync(newExam);
            return _mapper.Map<ExamDTO>(newExam);
        }

        public async Task<ExamDTO> CreateRandomAsync(ExamRandomCreateDTO dto)
        {
            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTimeHelper.GetVietnamNow();

            var randomQuestionIds = await BuildRandomExamQuestionIdsAsync(dto.RandomQuestionConfig);

            var newExam = new Exam
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                Title = dto.Title,
                Description = dto.Description,
                Duration = dto.Duration,
                PassScore = dto.PassScore,
                IsRandom = true,
                CreateAt = now,
                UpdateAt = now,
                Status = 1,
                ExamQuestions = randomQuestionIds.Select(questionId => new ExamQuestion
                {
                    ExamId = Guid.Empty,
                    QuestionId = questionId,
                    CreateAt = now,
                    UpdateAt = now,
                    Status = 1
                }).ToList()
            };

            foreach (var examQuestion in newExam.ExamQuestions)
            {
                examQuestion.ExamId = newExam.Id;
            }

            await _examRepository.AddAsync(newExam);
            return _mapper.Map<ExamDTO>(newExam);
        }

        private async Task<List<Guid>> BuildRandomExamQuestionIdsAsync(RandomExamQuestionConfigDTO? randomConfig)
        {
            var allQuestions = (await _questionRepository.GetAllAsync(status: 1)).ToList();

            if (!allQuestions.Any())
                throw ApiException.BadRequest("Không có câu hỏi trong hệ thống.");

            var criticalTag = (await _tagRepository.GetAllAsync(name: "Điểm liệt", status: 1))
                .FirstOrDefault(x => string.Equals(x.Name?.Trim(), "Điểm liệt", StringComparison.OrdinalIgnoreCase));

            if (criticalTag == null)
                throw ApiException.BadRequest("Không tìm thấy tag 'Điểm liệt'.");

            var criticalQuestionIds = allQuestions
                .Where(q => q.QuestionTags.Any(qt => qt.Status != 0 && qt.TagId == criticalTag.Id))
                .Select(q => q.Id)
                .ToHashSet();

            if (criticalQuestionIds.Count == 0)
                throw ApiException.BadRequest("Không có câu hỏi nào gắn tag 'Điểm liệt'.");

            if (randomConfig == null)
            {
                const int defaultTotalQuestions = 30;
                if (allQuestions.Count < defaultTotalQuestions)
                    throw ApiException.BadRequest($"Không đủ {defaultTotalQuestions} câu hỏi để random.");

                var selectedIds = PickRandomQuestionIds(allQuestions.Select(q => q.Id).ToList(), defaultTotalQuestions);
                EnsureContainsCriticalQuestion(selectedIds, allQuestions.Select(q => q.Id).ToList(), criticalQuestionIds);
                return selectedIds;
            }

            if (!randomConfig.TotalQuestions.HasValue || randomConfig.TotalQuestions.Value <= 0)
                throw ApiException.BadRequest("Tổng số câu hỏi phải lớn hơn 0.");

            if (randomConfig.ChapterRatios == null || !randomConfig.ChapterRatios.Any())
                throw ApiException.BadRequest("Phải truyền danh sách chapter và phần trăm.");

            var normalizedRatios = randomConfig.ChapterRatios
                .GroupBy(x => x.ChapterId)
                .Select(g => new RandomExamQuestionChapterRatioDTO
                {
                    ChapterId = g.Key,
                    Percentage = g.Sum(x => x.Percentage ?? 0)
                })
                .ToList();

            var totalPercentage = normalizedRatios.Sum(x => x.Percentage ?? 0);
            if (totalPercentage != 100)
                throw ApiException.BadRequest("Tổng phần trăm theo chapter phải bằng đúng 100%.");

            var totalQuestions = randomConfig.TotalQuestions.Value;
            var chapterRequiredCounts = AllocateQuestionCounts(totalQuestions, normalizedRatios);

            var selectedByChapter = new Dictionary<Guid, List<Guid>>();
            var chapterPools = new Dictionary<Guid, List<Guid>>();

            foreach (var ratio in normalizedRatios)
            {
                var chapterId = ratio.ChapterId;
                var requiredCount = chapterRequiredCounts[chapterId];

                var chapterPool = allQuestions
                    .Where(q => q.QuestionLesson?.QuestionChapterId == chapterId)
                    .Select(q => q.Id)
                    .Distinct()
                    .ToList();

                if (chapterPool.Count < requiredCount)
                    throw ApiException.BadRequest($"Chapter {chapterId} không đủ câu hỏi để lấy {requiredCount} câu.");

                chapterPools[chapterId] = chapterPool;
                selectedByChapter[chapterId] = PickRandomQuestionIds(chapterPool, requiredCount);
            }

            var finalSelectedIds = selectedByChapter
                .SelectMany(x => x.Value)
                .ToList();

            if (finalSelectedIds.Count != totalQuestions)
                throw ApiException.BadRequest("Không thể random đúng số lượng câu hỏi yêu cầu.");

            if (!finalSelectedIds.Any(id => criticalQuestionIds.Contains(id)))
            {
                var replaced = false;

                foreach (var ratio in normalizedRatios)
                {
                    var chapterId = ratio.ChapterId;
                    var selectedIds = selectedByChapter[chapterId];
                    if (!selectedIds.Any())
                        continue;

                    var criticalCandidate = chapterPools[chapterId]
                        .Where(id => criticalQuestionIds.Contains(id) && !selectedIds.Contains(id))
                        .FirstOrDefault();

                    if (criticalCandidate == Guid.Empty)
                        continue;

                    var nonCriticalSelectedIndex = selectedIds.FindIndex(id => !criticalQuestionIds.Contains(id));
                    if (nonCriticalSelectedIndex < 0)
                        continue;

                    selectedIds[nonCriticalSelectedIndex] = criticalCandidate;
                    replaced = true;
                    break;
                }

                if (!replaced)
                    throw ApiException.BadRequest("Không thể đảm bảo có ít nhất 1 câu 'Điểm liệt' theo cấu hình chapter hiện tại.");

                finalSelectedIds = selectedByChapter.SelectMany(x => x.Value).ToList();
            }

            return finalSelectedIds;
        }

        private static Dictionary<Guid, int> AllocateQuestionCounts(
            int totalQuestions,
            List<RandomExamQuestionChapterRatioDTO> chapterRatios)
        {
            var parts = chapterRatios
                .Select(x =>
                {
                    var percentage = x.Percentage ?? 0;
                    var exact = totalQuestions * percentage / 100m;
                    var floor = (int)Math.Floor(exact);
                    return new
                    {
                        x.ChapterId,
                        Floor = floor,
                        Fraction = exact - floor
                    };
                })
                .ToList();

            var result = parts.ToDictionary(x => x.ChapterId, x => x.Floor);
            var allocated = result.Values.Sum();
            var remainder = totalQuestions - allocated;

            if (remainder > 0)
            {
                var order = parts
                    .OrderByDescending(x => x.Fraction)
                    .ThenBy(x => x.ChapterId)
                    .ToList();

                for (var i = 0; i < remainder; i++)
                {
                    var chapterId = order[i % order.Count].ChapterId;
                    result[chapterId]++;
                }
            }

            return result;
        }

        private static List<Guid> PickRandomQuestionIds(List<Guid> pool, int count)
        {
            return pool
                .OrderBy(_ => Random.Shared.Next())
                .Take(count)
                .ToList();
        }

        private static void EnsureContainsCriticalQuestion(
            List<Guid> selectedIds,
            List<Guid> allPoolIds,
            HashSet<Guid> criticalQuestionIds)
        {
            if (selectedIds.Any(id => criticalQuestionIds.Contains(id)))
                return;

            var criticalCandidate = allPoolIds
                .Where(id => criticalQuestionIds.Contains(id) && !selectedIds.Contains(id))
                .FirstOrDefault();

            if (criticalCandidate == Guid.Empty)
                throw ApiException.BadRequest("Không thể đảm bảo có ít nhất 1 câu 'Điểm liệt'.");

            var nonCriticalIndex = selectedIds.FindIndex(id => !criticalQuestionIds.Contains(id));
            if (nonCriticalIndex < 0)
                throw ApiException.BadRequest("Không thể đảm bảo có ít nhất 1 câu 'Điểm liệt'.");

            selectedIds[nonCriticalIndex] = criticalCandidate;
        }

        public async Task<ExamDTO> UpdateAsync(Guid id, ExamUpdateDTO dto)
        {
            var existing = await _examRepository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound("Không tìm thấy exam");

            var currentUserId = UserContextHelper.GetRequiredCurrentUserId(_httpContextAccessor);
            var now = DateTimeHelper.GetVietnamNow();

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
                        throw ApiException.BadRequest($"ExamQuestion.ExamId ({examQuestionDto.ExamId}) không khớp Exam Id ({id}).");

                    var question = await _questionRepository.GetByIdAsync(examQuestionDto.QuestionId);

                    if (question == null)
                        throw ApiException.NotFound("Question không tồn tại");

                    if (examQuestionDto.Id.HasValue)
                    {
                        if (!existingExamQuestionsById.TryGetValue(examQuestionDto.Id.Value, out var examQuestion))
                            throw ApiException.NotFound($"Không tìm thấy ExamQuestion với Id {examQuestionDto.Id.Value}");

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
            return _mapper.Map<ExamDTO>(existing);
        }

        public async Task<ExamDTO> DeleteSoftAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var exam = await _examRepository.GetByIdAsync(id, role);
            if (exam == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            await _examRepository.DeleteSoftAsync(id);
            exam.Status = 0;
            exam.UpdateAt = DateTimeHelper.GetVietnamNow();
            return _mapper.Map<ExamDTO>(exam);
        }

        public async Task<ExamDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var exam = await _examRepository.GetByIdAsync(id, role);
            if (exam == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<ExamDTO>(exam);
            await _examRepository.DeleteHardAsync(id);
            return result;
        }

    }
}
