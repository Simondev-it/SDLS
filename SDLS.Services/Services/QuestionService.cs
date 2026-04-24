using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Answer;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.Question;
using SDLS.Model.DTOs.QuestionTag;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.Constants;
using SDLS.Model.DTOs.Notification;

namespace SDLS.Services.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionTopicRepository _questionTopicRepository;
        private readonly IQuestionLessonRepository _questionLessonRepository;
        private readonly IQuestionCategoryRepository _questionCategoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;
        private readonly IExecutionStrategyRepository _executionStrategy;
        private readonly AppDbContext _dbContext;

        private readonly Guid CREATE_TAG_ID = Guid.Parse("763a5be4-963a-487d-a3b4-6a826026c94e");
        private readonly Guid UPDATE_TAG_ID = Guid.Parse("8317546b-0cc6-43e9-a917-0ae9d090ec16");

        public QuestionService(
            IQuestionRepository questionRepository,
            IQuestionTopicRepository questionTopicRepository,
            IQuestionLessonRepository questionLessonRepository,
            IQuestionCategoryRepository questionCategoryRepository,
            ITagRepository tagRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper, 
            INotificationService notificationService,
            IExecutionStrategyRepository executionStrategy,
            AppDbContext dbContext)
        {
            _questionRepository = questionRepository;
            _questionTopicRepository = questionTopicRepository;
            _questionLessonRepository = questionLessonRepository;
            _questionCategoryRepository = questionCategoryRepository;
            _tagRepository = tagRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _notificationService = notificationService;
            _executionStrategy = executionStrategy;
            _dbContext = dbContext;
        }


        public async Task<PagedResult<QuestionDTO>> GetAllAsync(
            Guid? lessonId = null,
            Guid? topicId = null,
            Guid? QuestionCategoryId = null,
            List<Guid>? tagIds = null,
            string? searchContent = null,
            int? status = null,
            int page = 1,
            int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var filteredQuestions = await _questionRepository.GetFilteredForListAsync(
                lessonId,
                topicId,
                QuestionCategoryId,
                tagIds,
                searchContent,
                status,
                role);

            var orderedList = BuildOrderedLinkedList(filteredQuestions);
            var total = orderedList.Count;

            var pagedEntities = orderedList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedDtos = _mapper.Map<List<QuestionDTO>>(pagedEntities);

            for (int i = 0; i < pagedDtos.Count; i++)
            {
                pagedDtos[i].Position = (page - 1) * pageSize + i + 1;
            }

            return new PagedResult<QuestionDTO>
            {
                Items = pagedDtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }


        public async Task<QuestionDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var question = await _questionRepository.GetByIdAsync(id, role);
            if (question == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            return _mapper.Map<QuestionDTO>(question);
        }

        public async Task<QuestionDTO> GetByIdForAdminAsync(Guid id)
        {
            var entity = await _questionRepository.GetByIdForAdminAsync(id);
            if (entity == null) throw ApiException.NotFound("Không tìm thấy câu hỏi.");
            return _mapper.Map<QuestionDTO>(entity);
        }


        public async Task<QuestionDTO> CreateAsync(QuestionCreateDTO dto)
        {
            return await _executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    // --- LOGIC CREATE CŨ CỦA BẠN ---
                    if (dto.Answers == null || !dto.Answers.Any())
                        throw ApiException.BadRequest("Question must have at least 1 answer");
                    if (!dto.Answers.Any(a => a.IsCorrect))
                        throw ApiException.BadRequest("At least one answer must be correct");

                    var lesson = await _questionLessonRepository.GetByIdAsync(dto.QuestionLessonId);
                    if (lesson == null) throw ApiException.BadRequest($"QuestionLessonId {dto.QuestionLessonId} không tồn tại.");

                    var topic = await _questionTopicRepository.GetByIdAsync(dto.QuestionTopicId);
                    if (topic == null) throw ApiException.BadRequest($"QuestionTopicId {dto.QuestionTopicId} không tồn tại.");

                    var category = await _questionCategoryRepository.GetByIdAsync(dto.QuestionCategoryId);
                    if (category == null) throw ApiException.BadRequest($"QuestionCategoryId {dto.QuestionCategoryId} không tồn tại.");

                    var now = DateTimeHelper.GetVietnamNow();
                    var newQuestion = _mapper.Map<Question>(dto);
                    newQuestion.Id = Guid.NewGuid();
                    newQuestion.CreateAt = now;
                    newQuestion.UpdateAt = now;
                    newQuestion.Status = 1;
                    newQuestion.Image = dto.Image;

                    if (dto.Index.HasValue) { newQuestion.Index = dto.Index.Value; }
                    else
                    {
                        var allActive = await _questionRepository.GetAllAsync(status: 1);
                        var maxIndex = allActive.Any() ? allActive.Max(q => q.Index ?? 0) : 0;
                        newQuestion.Index = maxIndex + 1;
                    }

                    foreach (var ans in newQuestion.Answers)
                    {
                        ans.QuestionId = newQuestion.Id;
                        ans.CreateAt = now; ans.UpdateAt = now; ans.Status = 1;
                    }

                    foreach (var questionTag in newQuestion.QuestionTags)
                    {
                        var tag = await _tagRepository.GetByIdAsync(questionTag.TagId);
                        if (tag == null) throw ApiException.BadRequest($"TagId {questionTag.TagId} không tồn tại.");
                        questionTag.QuestionId = newQuestion.Id;
                        questionTag.CreateAt = now; questionTag.UpdateAt = now; questionTag.Status = 1;
                    }
                    newQuestion.ParentId = null;

                    await _questionRepository.AddAsync(newQuestion);
                    await RebuildGlobalParentLinksAsync(now);

                    // --- LOGIC NOTIFICATION ---
                    var adminIds = await _dbContext.Users
                        .AsNoTracking()
                        .Where(x => x.RoleId == RoleConst.ADMIN_ROLE_ID && x.Status != 0)
                        .Select(x => x.Id).ToListAsync();

                    if (adminIds.Any())
                    {
                        await _notificationService.CreateAsync(new NotificationCreateDTO
                        {
                            Title = "Câu hỏi mới",
                            Content = $"Có câu hỏi mới vừa được tạo cần duyệt.",
                            Status = 2,
                            UserNotifications = adminIds.Select(userId => new UserNotificationCreateDTO { UserId = userId }).ToList()
                        });
                    }

                    await transaction.CommitAsync();
                    return _mapper.Map<QuestionDTO>(newQuestion);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<List<QuestionDTO>> CreateManyAsync(List<QuestionCreateDTO> dtos)
        {
            if (dtos == null || dtos.Count == 0)
                throw ApiException.BadRequest("Danh sách câu hỏi không được rỗng.");

            await using var transaction = await _questionRepository.BeginTransactionAsync();
            try
            {
                var createdItems = new List<QuestionDTO>();

                foreach (var dto in dtos)
                {
                    var created = await CreateAsync(dto);
                    createdItems.Add(created);
                }

                await transaction.CommitAsync();
                return createdItems;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public Task<(byte[] Content, string FileName, string ContentType)> GenerateImportTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Questions");

            var headers = new[]
            {
                "QuestionLessonId",
                "QuestionTopicId",
                "QuestionCategoryId",
                "Index",
                "Content",
                "Image",
                "Explanation",
                "Type",
                "Answers",
                "QuestionTagIds"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            worksheet.Cell(2, 1).Value = "00000000-0000-0000-0000-000000000001";
            worksheet.Cell(2, 2).Value = "00000000-0000-0000-0000-000000000002";
            worksheet.Cell(2, 3).Value = "00000000-0000-0000-0000-000000000003";
            worksheet.Cell(2, 4).Value = "1";
            worksheet.Cell(2, 5).Value = "Nội dung câu hỏi mẫu";
            worksheet.Cell(2, 6).Value = "https://example.com/question-image.png";
            worksheet.Cell(2, 7).Value = "Giải thích mẫu";
            worksheet.Cell(2, 8).Value = "single";
            worksheet.Cell(2, 9).Value = "Đáp án A|true;Đáp án B|false";
            worksheet.Cell(2, 10).Value = "00000000-0000-0000-0000-000000000010;00000000-0000-0000-0000-000000000011";

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return Task.FromResult((
                stream.ToArray(),
                "question-import-template.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }

        public async Task<List<QuestionDTO>> ImportAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw ApiException.BadRequest("File không hợp lệ hoặc rỗng.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx")
                throw ApiException.BadRequest("Chỉ hỗ trợ file .xlsx");

            var rows = await ParseQuestionRowsFromXlsxAsync(file);
            if (!rows.Any())
                throw ApiException.BadRequest("Không có dữ liệu hợp lệ để import.");

            var dtos = new List<QuestionCreateDTO>();
            foreach (var row in rows)
            {
                var dto = BuildQuestionCreateDto(row.Data, row.RowNo);
                dtos.Add(dto);
            }

            return await CreateManyAsync(dtos);
        }

        public async Task<QuestionDTO> UpdateAsync(Guid id, QuestionUpdateDTO dto)
        {
            return await _executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    var existing = await _questionRepository.GetByIdForUpdateAsync(id);
                    if (existing == null) throw ApiException.NotFound("Không tìm thấy câu hỏi");

                    var lesson = await _questionLessonRepository.GetByIdAsync(dto.QuestionLessonId);
                    if (lesson == null) throw ApiException.BadRequest($"QuestionLessonId {dto.QuestionLessonId} không tồn tại.");

                    var topic = await _questionTopicRepository.GetByIdAsync(dto.QuestionTopicId);
                    if (topic == null) throw ApiException.BadRequest($"QuestionTopicId {dto.QuestionTopicId} không tồn tại.");

                    var category = await _questionCategoryRepository.GetByIdAsync(dto.QuestionCategoryId);
                    if (category == null) throw ApiException.BadRequest($"QuestionCategoryId {dto.QuestionCategoryId} không tồn tại.");

                    var now = DateTimeHelper.GetVietnamNow();
                    existing.QuestionLessonId = dto.QuestionLessonId;
                    existing.QuestionTopicId = dto.QuestionTopicId;
                    existing.QuestionCategoryId = dto.QuestionCategoryId;
                    existing.Index = dto.Index ?? existing.Index;
                    existing.Content = dto.Content;
                    existing.Image = dto.Image;
                    existing.Explanation = dto.Explanation;
                    existing.Type = dto.Type;
                    existing.Status = dto.Status ?? existing.Status;
                    existing.UpdateAt = now;

                    // Xử lý Answers
                    if (dto.Answers != null)
                    {
                        var existingAnswersById = existing.Answers.ToDictionary(a => a.Id, a => a);
                        foreach (var answerDto in dto.Answers)
                        {
                            if (answerDto.QuestionId != id) throw ApiException.BadRequest("Answer.QuestionId không khớp.");
                            if (answerDto.Id.HasValue)
                            {
                                if (!existingAnswersById.TryGetValue(answerDto.Id.Value, out var answer))
                                    throw ApiException.NotFound($"Không tìm thấy Answer {answerDto.Id.Value}");
                                answer.Content = answerDto.Content;
                                answer.IsCorrect = answerDto.IsCorrect;
                                answer.UpdateAt = now;
                                answer.Status = answerDto.Status ?? answer.Status ?? 1;
                            }
                            else
                            {
                                existing.Answers.Add(new Answer { QuestionId = id, Content = answerDto.Content, IsCorrect = answerDto.IsCorrect, CreateAt = now, UpdateAt = now, Status = answerDto.Status ?? 1 });
                            }
                        }
                    }

                    // Xử lý Tags
                    if (dto.QuestionTags != null)
                    {
                        // Lọc danh sách Tag từ DTO: Loại bỏ CREATE_TAG_ID và UPDATE_TAG_ID 
                        // vì chúng ta muốn Repository tự quản lý 2 tag này.
                        var newTagIds = dto.QuestionTags
                            .Select(qt => qt.TagId)
                            .Where(tagId => tagId != Guid.Empty && tagId != CREATE_TAG_ID && tagId != UPDATE_TAG_ID)
                            .Distinct()
                            .ToList();

                        // Xóa sạch các tag cũ (Nên gán lại list mới thay vì RemoveRange thủ công nếu dùng EF Core Tracking)
                        existing.QuestionTags.Clear();

                        foreach (var tagId in newTagIds)
                        {
                            var tag = await _tagRepository.GetByIdAsync(tagId);
                            if (tag == null) throw ApiException.BadRequest($"TagId {tagId} không tồn tại.");

                            existing.QuestionTags.Add(new QuestionTag
                            {
                                QuestionId = id,
                                TagId = tagId,
                                CreateAt = now,
                                UpdateAt = now,
                                Status = 1
                            });
                        }
                    }

                    await _questionRepository.UpdateAsync(existing);
                    await RebuildGlobalParentLinksAsync(now);

                    var adminIds = await _dbContext.Users
                        .AsNoTracking()
                        .Where(x => x.RoleId == RoleConst.ADMIN_ROLE_ID && x.Status != 0)
                        .Select(x => x.Id).ToListAsync();

                    if (adminIds.Any())
                    {
                        await _notificationService.CreateAsync(new NotificationCreateDTO
                        {
                            Title = "Câu hỏi cập nhật",
                            Content = $"Câu hỏi '{existing.Content.Substring(0, Math.Min(20, existing.Content.Length))}...' vừa được cập nhật.",
                            Status = 2,
                            UserNotifications = adminIds.Select(userId => new UserNotificationCreateDTO { UserId = userId }).ToList()
                        });
                    }

                    await transaction.CommitAsync();
                    return _mapper.Map<QuestionDTO>(existing);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<QuestionDTO> DeleteSoftAsync(Guid id)
        {
            var existing = await _questionRepository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw ApiException.NotFound($"Không tìm thấy câu hỏi với Id {id}");

            if (existing.Status != 0 && existing.Status != 1)
                throw ApiException.BadRequest("Chỉ hỗ trợ chuyển trạng thái giữa 0 và 1.");

            var now = DateTimeHelper.GetVietnamNow();
            var nextStatus = existing.Status == 0 ? 1 : 0;

            existing.Status = nextStatus;
            existing.UpdateAt = now;
            if (nextStatus == 0)
                existing.ParentId = null;

            await _questionRepository.UpdateAsync(existing);
            await RebuildGlobalParentLinksAsync(now);
            return _mapper.Map<QuestionDTO>(existing);
        }

        public async Task<QuestionDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var existing = await _questionRepository.GetByIdAsync(id, role);
            if (existing == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<QuestionDTO>(existing);
            await _questionRepository.DeleteHardAsync(id);
            return result;
        }


        


        // private method

        private List<Question> BuildOrderedLinkedList(IEnumerable<Question> all)
        {
            return all
                .OrderBy(q => q.Index ?? int.MaxValue)
                .ThenBy(q => q.CreateAt ?? DateTime.MinValue)
                .ThenBy(q => q.Id)
                .ToList();
        }

        private async Task RebuildGlobalParentLinksAsync(DateTime now)
        {
            var allActive = await _questionRepository.GetAllAsync(status: 1);
            var ordered = allActive
                .OrderBy(q => q.Index ?? int.MaxValue)
                .ThenBy(q => q.CreateAt ?? DateTime.MinValue)
                .ThenBy(q => q.Id)
                .ToList();

            if (!ordered.Any())
                return;

            var changedTracked = new List<Question>();

            for (var i = 0; i < ordered.Count; i++)
            {
                var currentId = ordered[i].Id;
                var expectedParentId = i + 1 < ordered.Count ? ordered[i + 1].Id : (Guid?)null;

                if (ordered[i].ParentId == expectedParentId)
                    continue;

                var tracked = await _questionRepository.GetByIdForUpdateAsync(currentId);
                if (tracked == null)
                    continue;

                tracked.ParentId = expectedParentId;
                tracked.UpdateAt = now;
                changedTracked.Add(tracked);
            }

            if (changedTracked.Any())
            {
                await _questionRepository.UpdateAsync(changedTracked[0]);
            }
        }

        private static QuestionCreateDTO BuildQuestionCreateDto(
            Dictionary<string, string> row,
            int rowNo)
        {
            var lessonIdRaw = GetRequired(row, "QuestionLessonId", rowNo);
            var topicIdRaw = GetRequired(row, "QuestionTopicId", rowNo);
            var categoryIdRaw = GetRequired(row, "QuestionCategoryId", rowNo);

            if (!Guid.TryParse(lessonIdRaw, out var lessonId) || lessonId == Guid.Empty)
                throw new ArgumentException($"QuestionLessonId không hợp lệ ở dòng {rowNo}.");

            if (!Guid.TryParse(topicIdRaw, out var topicId) || topicId == Guid.Empty)
                throw new ArgumentException($"QuestionTopicId không hợp lệ ở dòng {rowNo}.");

            if (!Guid.TryParse(categoryIdRaw, out var categoryId) || categoryId == Guid.Empty)
                throw new ArgumentException($"QuestionCategoryId không hợp lệ ở dòng {rowNo}.");

            var answersRaw = GetRequired(row, "Answers", rowNo);
            var type = GetRequired(row, "Type", rowNo);

            var dto = new QuestionCreateDTO
            {
                QuestionLessonId = lessonId,
                QuestionTopicId = topicId,
                QuestionCategoryId = categoryId,
                Index = ParseIntOptional(row, "Index"),
                Content = GetRequired(row, "Content", rowNo),
                Image = GetOptional(row, "Image"),
                Explanation = GetOptional(row, "Explanation"),
                Type = type,
                Answers = ParseAnswers(answersRaw),
                QuestionTags = ParseQuestionTags(GetOptional(row, "QuestionTagIds"))
            };

            return dto;
        }

        private static List<AnswerCreateDTO> ParseAnswers(string raw)
        {
            var result = new List<AnswerCreateDTO>();

            foreach (var part in raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var pieces = part.Split('|', 2, StringSplitOptions.TrimEntries);
                if (pieces.Length != 2)
                    throw new ArgumentException("Cột Answers sai định dạng. Dùng: Content|true;Content|false");

                if (!bool.TryParse(pieces[1], out var isCorrect))
                    throw new ArgumentException("IsCorrect trong cột Answers phải là true/false.");

                result.Add(new AnswerCreateDTO
                {
                    Content = pieces[0],
                    IsCorrect = isCorrect
                });
            }

            return result;
        }

        private static List<QuestionTagCreateDTO> ParseQuestionTags(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return new List<QuestionTagCreateDTO>();

            var tags = new List<QuestionTagCreateDTO>();
            foreach (var part in raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (!Guid.TryParse(part, out var tagId) || tagId == Guid.Empty)
                    throw new ArgumentException($"TagId không hợp lệ: '{part}'.");

                tags.Add(new QuestionTagCreateDTO { TagId = tagId });
            }

            return tags;
        }

        private static int? ParseIntOptional(Dictionary<string, string> row, string key)
        {
            var raw = GetOptional(row, key);
            if (string.IsNullOrWhiteSpace(raw)) return null;
            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : null;
        }

        private static string GetRequired(Dictionary<string, string> row, string key, int rowNo)
        {
            if (!row.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Thiếu cột bắt buộc {key}.");
            return value.Trim();
        }

        private static string? GetOptional(Dictionary<string, string> row, string key)
        {
            return row.TryGetValue(key, out var value) ? value?.Trim() : null;
        }

        private static string NormalizeLookupKey(string? value)
        {
            return (value ?? string.Empty).Trim().ToLowerInvariant();
        }

        private static async Task<List<(Dictionary<string, string> Data, int RowNo)>> ParseQuestionRowsFromXlsxAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault()
                ?? throw ApiException.BadRequest("File Excel không có worksheet.");

            var firstRow = worksheet.FirstRowUsed();
            if (firstRow == null)
                throw ApiException.BadRequest("File Excel không có header.");

            var headers = firstRow.CellsUsed()
                .Select(c => c.GetString()?.Trim() ?? string.Empty)
                .ToList();

            var requiredHeaders = new[]
            {
                "QuestionLessonId",
                "QuestionTopicId",
                "QuestionCategoryId",
                "Content",
                "Type",
                "Answers"
            };

            foreach (var required in requiredHeaders)
            {
                if (!headers.Any(h => string.Equals(h, required, StringComparison.OrdinalIgnoreCase)))
                    throw ApiException.BadRequest($"Thiếu cột bắt buộc: {required}.");
            }

            var result = new List<(Dictionary<string, string> Data, int RowNo)>();

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                if (row.CellsUsed().All(c => string.IsNullOrWhiteSpace(c.GetString())))
                    continue;

                var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < headers.Count; i++)
                {
                    var key = headers[i];
                    if (string.IsNullOrWhiteSpace(key))
                        continue;

                    data[key] = row.Cell(i + 1).GetString();
                }

                result.Add((data, row.RowNumber()));
            }

            return result;
        }
    }
}
