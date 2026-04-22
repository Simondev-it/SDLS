using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.Helpers;
using SDLS.Model.DTOs.QuestionLesson;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;
using SDLS.Services.Utilities;

namespace SDLS.Services.Services
{
    public class QuestionLessonService : IQuestionLessonService
    {
        private readonly IQuestionLessonRepository _repository;
        private readonly IQuestionChapterRepository _questionChapterRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILessonImageService _lessonImageService;

        public QuestionLessonService(
            IQuestionLessonRepository repository,
            IQuestionChapterRepository questionChapterRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ILessonImageService lessonImageService)
        {
            _repository = repository;
            _questionChapterRepository = questionChapterRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _lessonImageService = lessonImageService;
        }

        public async Task<PagedResult<QuestionLessonDTO>> GetAllAsync(
            Guid? id = null,
            Guid? questionChapterId = null,
            string? name = null,
            string? description = null,
            string? content = null,
            int? status = null,
            int page = 1,
            int pageSize = 20)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var filtered = await _repository.GetAllAsync(
                id, questionChapterId, name, description, content, status, role);

            var total = filtered.Count();

            var pageEntities = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var lessonIds = pageEntities.Select(x => x.Id).ToList();
            var lessonImages = await _repository.GetLessonImagesByLessonIdsAsync(lessonIds, role);
            var imageLookup = lessonImages.GroupBy(x => x.QuestionLessonId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var dtos = _mapper.Map<List<QuestionLessonDTO>>(pageEntities);

            foreach (var dto in dtos)
            {
                if (imageLookup.TryGetValue(dto.Id, out var images))
                    dto.LessonImages = _mapper.Map<List<QuestionLessonImageDTO>>(images);
            }

            return new PagedResult<QuestionLessonDTO>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<QuestionLessonDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var lesson = await _repository.GetByIdAsync(id, role);
            if (lesson == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var dto = _mapper.Map<QuestionLessonDTO>(lesson);
            var images = await _repository.GetLessonImagesByLessonIdsAsync(new List<Guid> { id }, role);
            dto.LessonImages = _mapper.Map<List<QuestionLessonImageDTO>>(images);

            return dto;
        }

        public async Task<QuestionLessonDTO> CreateAsync(QuestionLessonCreateDTO dto)
        {
            if (dto.QuestionChapterId == Guid.Empty)
                throw ApiException.BadRequest("QuestionChapterId không hợp lệ.");

            var chapter = await _questionChapterRepository.GetByIdAsync(dto.QuestionChapterId);
            if (chapter == null)
                throw ApiException.BadRequest("Không tìm thấy QuestionChapter với Id " + dto.QuestionChapterId);

            var now = DateTimeHelper.GetVietnamNow();

            var lesson = new QuestionLesson
            {
                Id = Guid.NewGuid(),
                QuestionChapterId = dto.QuestionChapterId,
                Index = dto.Index,
                Name = dto.Name,
                Description = dto.Description,
                Content = dto.Content,
                CreateAt = now,
                UpdateAt = now,
                Status = 1
            };

            await _repository.AddAsync(lesson);

            await SyncLessonImagesFromContentAsync(lesson.Id, dto.Content, now);
            await _repository.UpdateAsync(lesson);

            return _mapper.Map<QuestionLessonDTO>(lesson);
        }

        public Task<(byte[] Content, string FileName, string ContentType)> GenerateImportTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("QuestionLessons");

            var headers = new[] { "QuestionChapterName", "Index", "Name", "Description", "Content" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return Task.FromResult((
                stream.ToArray(),
                "question-lesson-import-template.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }

        public async Task<List<QuestionLessonDTO>> ImportAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw ApiException.BadRequest("File không hợp lệ hoặc rỗng.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx")
                throw ApiException.BadRequest("Chỉ hỗ trợ file .xlsx");

            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var chapters = await _questionChapterRepository.GetAllAsync(status: 1, role: role);
            var chapterLookup = chapters
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .GroupBy(x => x.Name!.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            var items = await ParseXlsxAsync(file, chapterLookup);
            if (items.Count == 0)
                throw ApiException.BadRequest("Không có dữ liệu hợp lệ để import.");

            var createdItems = new List<QuestionLessonDTO>();
            foreach (var item in items)
            {
                var created = await CreateAsync(item);
                createdItems.Add(created);
            }

            return createdItems;
        }

        public async Task<(byte[] Content, string FileName, string ContentType)> ExportToExcelAsync(
            Guid? id = null,
            Guid? questionChapterId = null,
            string? name = null,
            string? description = null,
            string? content = null,
            int? status = null)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var items = await _repository.GetAllAsync(id, questionChapterId, name, description, content, status, role);
            var ordered = items
                .OrderByDescending(x => x.UpdateAt ?? x.CreateAt ?? DateTime.MinValue)
                .ThenByDescending(x => x.CreateAt ?? DateTime.MinValue)
                .ThenByDescending(x => x.Id)
                .ToList();

            var lessonIds = ordered.Select(x => x.Id).ToList();
            var lessonImages = await _repository.GetLessonImagesByLessonIdsAsync(lessonIds, role);
            var imageLookup = lessonImages
                .GroupBy(x => x.QuestionLessonId)
                .ToDictionary(g => g.Key, g => _mapper.Map<List<QuestionLessonImageDTO>>(g.ToList()));

            var dtos = _mapper.Map<List<QuestionLessonDTO>>(ordered);
            foreach (var dto in dtos)
            {
                if (imageLookup.TryGetValue(dto.Id, out var images))
                    dto.LessonImages = images;
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("QuestionLessons");

            var headers = new[]
            {
                "Id",
                "QuestionChapterId",
                "QuestionChapterName",
                "QuestionChapterIndex",
                "QuestionChapterStatus",
                "Index",
                "Name",
                "Description",
                "Content",
                "Status",
                "CreateAt",
                "UpdateAt",
                "LessonImageCount",
                "LessonImageIds",
                "LessonImageNames",
                "LessonImageUrls",
                "LessonImageStatuses",
                "LessonImageCreateAts",
                "LessonImageUpdateAts"
            };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            for (int row = 0; row < dtos.Count; row++)
            {
                var item = dtos[row];
                var r = row + 2;

                worksheet.Cell(r, 1).Value = item.Id.ToString();
                worksheet.Cell(r, 2).Value = item.QuestionChapterId.ToString();
                worksheet.Cell(r, 3).Value = item.QuestionChapter?.Name ?? string.Empty;
                worksheet.Cell(r, 4).Value = item.QuestionChapter?.Index;
                worksheet.Cell(r, 5).Value = item.QuestionChapter?.Status;
                worksheet.Cell(r, 6).Value = item.Index;
                worksheet.Cell(r, 7).Value = item.Name;
                worksheet.Cell(r, 8).Value = item.Description ?? string.Empty;
                worksheet.Cell(r, 9).Value = item.Content ?? string.Empty;
                worksheet.Cell(r, 10).Value = item.Status;
                worksheet.Cell(r, 11).Value = item.CreateAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
                worksheet.Cell(r, 12).Value = item.UpdateAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
                worksheet.Cell(r, 13).Value = item.LessonImages.Count;
                worksheet.Cell(r, 14).Value = string.Join(" | ", item.LessonImages.Select(x => x.Id));
                worksheet.Cell(r, 15).Value = string.Join(" | ", item.LessonImages.Select(x => x.Name));
                worksheet.Cell(r, 16).Value = string.Join(" | ", item.LessonImages.Select(x => x.Url));
                worksheet.Cell(r, 17).Value = string.Join(" | ", item.LessonImages.Select(x => x.Status));
                worksheet.Cell(r, 18).Value = string.Join(" | ", item.LessonImages.Select(x => x.CreateAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty));
                worksheet.Cell(r, 19).Value = string.Join(" | ", item.LessonImages.Select(x => x.UpdateAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty));
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return (
                stream.ToArray(),
                "question-lessons.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<QuestionLessonDTO> UpdateAsync(Guid id, QuestionLessonUpdateDTO dto)
        {
            var lesson = await _repository.GetByIdForUpdateAsync(id);
            if (lesson == null)
                throw ApiException.NotFound("Không tìm thấy QuestionLesson");

            var now = DateTimeHelper.GetVietnamNow();
            var changed = false;

            if (dto.QuestionChapterId.HasValue)
            {
                if (dto.QuestionChapterId.Value == Guid.Empty)
                    throw ApiException.BadRequest("QuestionChapterId không hợp lệ.");

                var chapter = await _questionChapterRepository.GetByIdAsync(dto.QuestionChapterId.Value);
                if (chapter == null)
                    throw ApiException.BadRequest("Không tìm thấy QuestionChapter với Id " + dto.QuestionChapterId);

                if (lesson.QuestionChapterId != dto.QuestionChapterId.Value)
                {
                    lesson.QuestionChapterId = dto.QuestionChapterId.Value;
                    changed = true;
                }
            }

            if (dto.Index.HasValue && lesson.Index != dto.Index.Value)
            {
                lesson.Index = dto.Index.Value;
                changed = true;
            }

            if (dto.Name != null)
            {
                var newName = dto.Name.Trim();
                if (string.IsNullOrWhiteSpace(newName))
                    throw ApiException.BadRequest("Name không được để trống.");

                if (!string.Equals(lesson.Name, newName, StringComparison.Ordinal))
                {
                    lesson.Name = newName;
                    changed = true;
                }
            }

            if (dto.Description != null && !string.Equals(lesson.Description, dto.Description, StringComparison.Ordinal))
            {
                lesson.Description = dto.Description;
                changed = true;
            }

            if (dto.Content != null && !string.Equals(lesson.Content, dto.Content, StringComparison.Ordinal))
            {
                lesson.Content = dto.Content;
                await SyncLessonImagesFromContentAsync(id, dto.Content, now);
                changed = true;
            }

            if (dto.Status.HasValue)
            {
                var nextStatus = dto.Status.Value;
                if (lesson.Status != nextStatus)
                {
                    lesson.Status = nextStatus;
                    changed = true;
                }
            }

            if (!changed)
                return _mapper.Map<QuestionLessonDTO>(lesson);

            lesson.UpdateAt = now;

            await _repository.UpdateAsync(lesson);
            return _mapper.Map<QuestionLessonDTO>(lesson);
        }

        public async Task<QuestionLessonDTO> DeleteAsync(Guid id)
        {
            return await DeleteSoftAsync(id);
        }

        public async Task<QuestionLessonDTO> DeleteSoftAsync(Guid id)
        {
            var lesson = await _repository.GetByIdForUpdateAsync(id);
            if (lesson == null)
                throw ApiException.NotFound($"Không tìm thấy QuestionLesson với Id {id}");

            var now = DateTimeHelper.GetVietnamNow();

            var currentStatus = lesson.Status ?? 1;
            var nextStatus = currentStatus == 0 ? 1 : 0;

            lesson.Status = nextStatus;
            lesson.UpdateAt = now;

            if (nextStatus == 0)
                await _repository.SoftDeleteLessonImagesAsync(id, now);
            else
                await _repository.RestoreLessonImagesAsync(id, now);

            await _repository.UpdateAsync(lesson);

            return _mapper.Map<QuestionLessonDTO>(lesson);
        }

        public async Task<QuestionLessonDTO> DeleteHardAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);
            var lesson = await _repository.GetByIdAsync(id, role);
            if (lesson == null)
                throw ApiException.NotFound($"Not found with ID {id}");

            var result = _mapper.Map<QuestionLessonDTO>(lesson);
            await _repository.DeleteHardAsync(id);
            return result;
        }

        private async Task SyncLessonImagesFromContentAsync(Guid lessonId, string? content, DateTime now)
        {
            var newUrls = HtmlContentParser.ExtractImageUrls(content);
            var activeImages = await _repository.GetLessonImagesByLessonIdForUpdateAsync(lessonId);

            var activeUrlSet = activeImages
                .Where(x => !string.IsNullOrWhiteSpace(x.Url))
                .Select(x => x.Url!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var imagesToRemove = activeImages
                .Where(x => !string.IsNullOrWhiteSpace(x.Url)
                    && !newUrls.Contains(x.Url!, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (imagesToRemove.Any())
            {
                foreach (var img in imagesToRemove)
                {
                    await _lessonImageService.DeleteAsync(img.Id);
                }
                _repository.RemoveLessonImages(imagesToRemove);
            }

            var imagesToAdd = new List<LessonImage>();
            foreach (var url in newUrls)
            {
                if (activeUrlSet.Contains(url))
                    continue;

                imagesToAdd.Add(new LessonImage
                {
                    Id = Guid.NewGuid(),
                    QuestionLessonId = lessonId,
                    Name = HtmlContentParser.ResolveImageNameFromUrl(url),
                    Url = url,
                    CreateAt = now,
                    UpdateAt = now,
                    Status = 1
                });
            }

            if (imagesToAdd.Any())
            {
                _repository.AddLessonImages(imagesToAdd);
            }
        }

        private static async Task<List<QuestionLessonCreateDTO>> ParseXlsxAsync(
            IFormFile file,
            Dictionary<string, Guid> chapterLookup)
        {
            await using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault()
                ?? throw ApiException.BadRequest("File Excel không có worksheet.");

            var firstRow = worksheet.FirstRowUsed();
            if (firstRow == null)
                throw ApiException.BadRequest("File Excel không có header.");

            var headerMap = BuildHeaderMap(firstRow.CellsUsed().Select(c => c.GetString()).ToList());
            var result = new List<QuestionLessonCreateDTO>();

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                if (row.CellsUsed().All(c => string.IsNullOrWhiteSpace(c.GetString())))
                    continue;

                var chapterName = GetCellValue(row, headerMap, "QuestionChapterName")?.Trim();
                if (string.IsNullOrWhiteSpace(chapterName) || !chapterLookup.TryGetValue(chapterName, out var chapterId))
                    throw ApiException.BadRequest($"Dòng {row.RowNumber()}: QuestionChapterName không tồn tại hoặc để trống.");

                var name = GetCellValue(row, headerMap, "Name")?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                    throw ApiException.BadRequest($"Dòng {row.RowNumber()}: Name là bắt buộc.");

                int? index = null;
                var indexRaw = GetCellValue(row, headerMap, "Index")?.Trim();
                if (!string.IsNullOrWhiteSpace(indexRaw) && int.TryParse(indexRaw, out var parsedIndex))
                    index = parsedIndex;

                result.Add(new QuestionLessonCreateDTO
                {
                    QuestionChapterId = chapterId,
                    Index = index,
                    Name = name,
                    Description = GetCellValue(row, headerMap, "Description")?.Trim(),
                    Content = GetCellValue(row, headerMap, "Content")
                });
            }

            return result;
        }

        private static Dictionary<string, int> BuildHeaderMap(List<string> headers)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < headers.Count; i++)
            {
                var key = NormalizeHeader(headers[i]);
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                map[key] = i;
            }

            var required = new[] { "questionchaptername", "name" };
            foreach (var key in required)
            {
                if (!map.ContainsKey(key))
                    throw ApiException.BadRequest($"Thiếu cột bắt buộc: {key}");
            }

            return map;
        }

        private static string? GetCellValue(IXLRow row, Dictionary<string, int> headerMap, string key)
        {
            var normalizedKey = NormalizeHeader(key);
            if (!headerMap.TryGetValue(normalizedKey, out var index))
                return null;

            return row.Cell(index + 1).GetString();
        }

        private static string NormalizeHeader(string? value)
        {
            return (value ?? string.Empty)
                .Trim()
                .Replace("_", string.Empty)
                .Replace(" ", string.Empty)
                .ToLowerInvariant();
        }

    }
}