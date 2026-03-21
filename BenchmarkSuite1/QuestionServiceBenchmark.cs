using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using SDLS.Model.AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Answer;
using SDLS.Model.DTOs.Question;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VSDiagnostics;

namespace SDLS.Services.Benchmarks
{
    [SimpleJob(warmupCount: 3, targetCount: 5)]
    [CPUUsageDiagnoser]
    public class QuestionServiceBenchmark
    {
        private QuestionService _questionService;
        private Mock<IQuestionRepository> _repositoryMock;
        private IMapper _mapper;
        private List<Question> _testQuestions;
        [GlobalSetup]
        public void Setup()
        {
            // Initialize AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
            // Create mock repository
            _repositoryMock = new Mock<IQuestionRepository>();
            // Generate test data - simulate a medium-sized dataset
            _testQuestions = GenerateTestQuestions(100);
            // Configure mock
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(_testQuestions);
            _questionService = new QuestionService(_repositoryMock.Object, _mapper);
        }

        [Benchmark]
        public async Task GetAllAsync_DefaultParameters()
        {
            await _questionService.GetAllAsync(lessonId: null, topicId: null, QuestionCategoryId: null, page: 1, pageSize: 20);
        }

        [Benchmark]
        public async Task GetAllAsync_WithLessonFilter()
        {
            var lessonId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            await _questionService.GetAllAsync(lessonId: lessonId, topicId: null, QuestionCategoryId: null, page: 1, pageSize: 20);
        }

        [Benchmark]
        public async Task GetAllAsync_MultipleFilters()
        {
            var lessonId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var topicId = Guid.Parse("00000000-0000-0000-0000-000000000011");
            await _questionService.GetAllAsync(lessonId: lessonId, topicId: topicId, QuestionCategoryId: null, page: 1, pageSize: 20);
        }

        private List<Question> GenerateTestQuestions(int count)
        {
            var questions = new List<Question>();
            Guid previousId = Guid.Empty;
            for (int i = 0; i < count; i++)
            {
                var questionId = Guid.NewGuid();
                var question = new Question
                {
                    Id = questionId,
                    Content = $"Question {i}",
                    Type = "multiple_choice",
                    QuestionLessonId = Guid.Parse("00000000-0000-0000-0000-00000000000" + (i % 3)),
                    QuestionTopicId = Guid.Parse("00000000-0000-0000-0000-00000000001" + (i % 5)),
                    QuestionCategoryId = Guid.Parse("00000000-0000-0000-0000-00000000002" + (i % 2)),
                    ParentId = i == 0 ? null : previousId,
                    Status = 1,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow,
                    Answers = new List<Answer>
                    {
                        new Answer
                        {
                            Id = Guid.NewGuid(),
                            Content = $"Answer 1 for Q{i}",
                            Iscorrect = true,
                            Status = 1,
                            QuestionId = questionId
                        },
                        new Answer
                        {
                            Id = Guid.NewGuid(),
                            Content = $"Answer 2 for Q{i}",
                            Iscorrect = false,
                            Status = 1,
                            QuestionId = questionId
                        }
                    }
                };
                questions.Add(question);
                previousId = questionId;
            }

            return questions;
        }
    }
}