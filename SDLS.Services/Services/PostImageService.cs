using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs.ForumPost;
using SDLS.Model.Enumerations;
using SDLS.Model.Models;
using SDLS.Repositories.Interface.ImageInterfaces;
using SDLS.Services.ApiExceptions;
using SDLS.Services.Interfaces;

namespace SDLS.Services.Services
{
    public class PostImageService : IPostImageService
    {
        private readonly IPostImageRepository _postImageRepository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public PostImageService(
            IPostImageRepository postImageRepository,
            IStorageService storageService,
            IMapper mapper,
            AppDbContext dbContext)
        {
            _postImageRepository = postImageRepository;
            _storageService = storageService;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ForumPostImageDTO>> GetAllAsync()
        {
            var images = await _postImageRepository.GetAllAsync();
            return _mapper.Map<List<ForumPostImageDTO>>(images);
        }

        public async Task<ForumPostImageDTO> GetByIdAsync(Guid id)
        {
            var image = await _postImageRepository.GetByIdAsync(id);
            if (image == null)
            {
                throw ApiException.NotFound($"Post image not found with ID {id}");
            }

            return _mapper.Map<ForumPostImageDTO>(image);
        }

        public async Task<IEnumerable<ForumPostImageDTO>> GetByPostIdAsync(Guid postId)
        {
            var images = await _postImageRepository.GetByPostIdAsync(postId);
            if (images == null || !images.Any())
            {
                throw ApiException.NotFound($"Post image not found for post ID {postId}");
            }

            return _mapper.Map<List<ForumPostImageDTO>>(images);
        }

        public async Task<ForumPostImageDTO> CreateAsync(IFormFile file, Guid postId, string? name = null)
        {
            if (file == null || file.Length == 0)
            {
                throw ApiException.BadRequest("Image file is required");
            }

            var postExists = await _dbContext.ForumPosts.AnyAsync(fp => fp.Id == postId);
            if (!postExists)
            {
                throw ApiException.NotFound($"ForumPost with ID {postId} not found");
            }

            var url = await _storageService.UploadImageAsync(file, ImageTarget.PostImage, postId);

            var image = new PostImage
            {
                Id = Guid.NewGuid(),
                ForumPostId = postId,
                Name = string.IsNullOrWhiteSpace(name) ? file.FileName : name,
                Url = url,
                CreateAt = DateTime.UtcNow.ToLocalTime(),
                UpdateAt = DateTime.UtcNow.ToLocalTime(),
                Status = 1
            };

            await _postImageRepository.AddAsync(image);

            return _mapper.Map<ForumPostImageDTO>(image);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var image = await _postImageRepository.GetByIdAsync(id);
            if (image == null)
            {
                throw ApiException.NotFound($"Post image not found with ID {id}");
            }

            if (!string.IsNullOrWhiteSpace(image.Url))
            {
                await _storageService.DeleteImageAsync(image.Url, ImageTarget.PostImage);
            }

            await _postImageRepository.DeleteAsync(id);
            return true;
        }
    }
}
