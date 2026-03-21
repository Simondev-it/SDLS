using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SDLS.Model.AutoMapper;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Repositories.Interface.ImageInterfaces;
using SDLS.Repositories.Repositories;
using SDLS.Services.Interfaces;
using SDLS.Services.Services;

namespace SDLS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);
                });
            });

            builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile<MappingProfile>();
            });

            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

            builder.Services.AddScoped<IExamService, ExamService>();
            builder.Services.AddScoped<IExamRepository, ExamRepository>();

            builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
            builder.Services.AddScoped<ILessonImageRepository, LessonImageRepository>();

            builder.Services.AddScoped<ILessonImageService, LessonImageService>();
            builder.Services.AddScoped<IStorageService, StorageService>();

            builder.Services.AddScoped<ILearningProgressRepository, LearningProgressRepository>();
            builder.Services.AddScoped<ILearningProgressService, LearningProgressService>();

            builder.Services.AddScoped<IExamSessionRepository, ExamSessionRepository>();
            builder.Services.AddScoped<IExamSessionService, ExamSessionService>();

            builder.Services.AddScoped<IQuestionChapterRepository, QuestionChapterRepository>();
            builder.Services.AddScoped<IQuestionChapterService, QuestionChapterService>();

            builder.Services.AddScoped<IQuestionLessonRepository, QuestionLessonRepository>();
            builder.Services.AddScoped<IQuestionLessonService, QuestionLessonService>();

            builder.Services.AddScoped<IDrivingLicenseRepository, DrivingLicenseRepository>();
            builder.Services.AddScoped<IDrivingLicenseService, DrivingLicenseService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Add supabase 
            var supabaseUrl = builder.Configuration["Supabase:Url"];
            var supabaseServiceRoleKey = builder.Configuration["Supabase:ServiceRoleKey"];
            var supabaseKey = string.IsNullOrWhiteSpace(supabaseServiceRoleKey)
                ? builder.Configuration["Supabase:Key"]
                : supabaseServiceRoleKey;

            if (string.IsNullOrWhiteSpace(supabaseUrl) || string.IsNullOrWhiteSpace(supabaseKey))
            {
                throw new InvalidOperationException("Supabase configuration is missing. Set Supabase:Url and Supabase:ServiceRoleKey (or Supabase:Key).");
            }

            builder.Services.AddScoped(_ => new Supabase.Client(supabaseUrl, supabaseKey));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
