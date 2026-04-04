using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs.LessonImage;
using SDLS.Model.Enumerations;
using SDLS.Model.Models;
using SDLS.Repositories.Interface.ImageInterfaces;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class LessonImageService : ILessonImageService
    {
        private readonly ILessonImageRepository _lessonImageRepository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public LessonImageService(
            ILessonImageRepository lessonImageRepository,
            IStorageService storageService,
            IMapper mapper,
            AppDbContext dbContext)
        {
            _lessonImageRepository = lessonImageRepository;
            _storageService = storageService;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<LessonImageDTO>> GetAllAsync()
        {
            var images = await _lessonImageRepository.GetAllAsync();
            return _mapper.Map<List<LessonImageDTO>>(images);
        }

        public async Task<LessonImageDTO> GetByIdAsync(Guid id)
        {
            var image = await _lessonImageRepository.GetByIdAsync(id);
            if (image == null)
            {
                throw ApiException.NotFound($"Lesson image not found with ID {id}");
            }

            return _mapper.Map<LessonImageDTO>(image);
        }

        public async Task<LessonImageDTO> GetByLessonIdAsync(Guid lessonId)
        {
            var image = await _lessonImageRepository.GetByLessonIdAsync(lessonId);
            if (image == null)
            {
                throw ApiException.NotFound($"Lesson image not found for lesson ID {lessonId}");
            }

            return _mapper.Map<LessonImageDTO>(image);
        }

        public async Task<LessonImageDTO> CreateAsync(IFormFile file, Guid lessonId, string? name = null)
        {
            if (file == null || file.Length == 0)
            {
                throw ApiException.BadRequest("Image file is required");
            }

            // Validate that the QuestionLesson exists
            var questionLessonExists = await _dbContext.QuestionLessons.AnyAsync(ql => ql.Id == lessonId);
            if (!questionLessonExists)
            {
                throw ApiException.NotFound($"QuestionLesson with ID {lessonId} not found");
            }

            var url = await _storageService.UploadImageAsync(file, ImageTarget.LessonImage, lessonId);

            var image = new LessonImage
            {
                Id = Guid.NewGuid(),
                QuestionLessonId = lessonId,
                Name = string.IsNullOrWhiteSpace(name) ? file.FileName : name,
                Url = url,
                CreateAt = DateTime.UtcNow.ToLocalTime(),
                UpdateAt = DateTime.UtcNow.ToLocalTime(),
                Status = 1
            };

            await _lessonImageRepository.AddAsync(image);

            return _mapper.Map<LessonImageDTO>(image);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var image = await _lessonImageRepository.GetByIdAsync(id);
            if (image == null)
            {
                throw ApiException.NotFound($"Lesson image not found with ID {id}");
            }

            if (!string.IsNullOrWhiteSpace(image.Url))
            {
                await _storageService.DeleteImageAsync(image.Url, ImageTarget.LessonImage);
            }

            await _lessonImageRepository.DeleteAsync(id);
            return true;
        }
    }
}
