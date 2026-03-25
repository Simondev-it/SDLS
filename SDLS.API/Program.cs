using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SDLS.Model.AutoMapper;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Repositories.Interface.ImageInterfaces;
using SDLS.Repositories.Repositories;
using SDLS.Services.Interfaces;
using SDLS.Services.Services;
using System.Text;

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

            builder.Services.AddScoped<IExecutionStrategyRepository, ExecutionStrategyRepository>();

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


            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IJwtService, JwtService>();

            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IForumPostRepository, ForumPostRepository>();
            builder.Services.AddScoped<IForumPostService, ForumPostService>();

            builder.Services.AddScoped<IMediaImageService, MediaImageService>();

            builder.Services.AddScoped<IDrivingLicenseRepository, DrivingLicenseRepository>();
            builder.Services.AddScoped<IDrivingLicenseService, DrivingLicenseService>();

            builder.Services.AddScoped<ITagRepository, TagRepository>();
            builder.Services.AddScoped<ITagService, TagService>();

            builder.Services.AddScoped<IQuestionCategoryRepository, QuestionCategoryRepository>();
            builder.Services.AddScoped<IQuestionCategoryService, QuestionCategoryService>();

            builder.Services.AddScoped<IQuestionTopicRepository, QuestionTopicRepository>();
            builder.Services.AddScoped<IQuestionTopicService, QuestionTopicService>();

            builder.Services.AddScoped<ISimulationDifficultyLevelRepository, SimulationDifficultyLevelRepository>();
            builder.Services.AddScoped<ISimulationDifficultyLevelService, SimulationDifficultyLevelService>();

            builder.Services.AddScoped<ISimulationCategoryRepository, SimulationCategoryRepository>();
            builder.Services.AddScoped<ISimulationCategoryService, SimulationCategoryService>();

            builder.Services.AddScoped<ISignCategoryRepository, SignCategoryRepository>();
            builder.Services.AddScoped<ISignCategoryService, SignCategoryService>();

            builder.Services.AddScoped<IReportCategoryRepository, ReportCategoryRepository>();
            builder.Services.AddScoped<IReportCategoryService, ReportCategoryService>();

            builder.Services.AddScoped<IForumTopicRepository, ForumTopicRepository>();
            builder.Services.AddScoped<IForumTopicService, ForumTopicService>();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Accept payloads that contain a trailing comma to reduce client-side parsing failures.
                    options.JsonSerializerOptions.AllowTrailingCommas = true;
                });

            // CORS: allow frontend dev server
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("LocalFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            builder.Services.AddScoped<ISimulationChapterRepository, SimulationChapterRepository>();
            builder.Services.AddScoped<ISimulationChapterService, SimulationChapterService>();

            builder.Services.AddScoped<IUserLicenseRepository, UserLicenseRepository>();
            builder.Services.AddScoped<IUserLicenseService, UserLicenseService>();

            builder.Services.AddScoped<ILessonProgressRepository, LessonProgressRepository>();
            builder.Services.AddScoped<ILessonProgressService, LessonProgressService>();

            builder.Services.AddScoped<ISavedQuestionRepository, SavedQuestionRepository>();
            builder.Services.AddScoped<ISavedQuestionService, SavedQuestionService>();

            builder.Services.AddScoped<ISavedTrafficSignRepository, SavedTrafficSignRepository>();
            builder.Services.AddScoped<ISavedTrafficSignService, SavedTrafficSignService>();

            builder.Services.AddScoped<IPostReactRepository, PostReactRepository>();
            builder.Services.AddScoped<IPostReactService, PostReactService>();

            builder.Services.AddScoped<ICommentVoteRepository, CommentVoteRepository>();
            builder.Services.AddScoped<ICommentVoteService, CommentVoteService>();

            builder.Services.AddScoped<IForumCommentRepository, ForumCommentRepository>();
            builder.Services.AddScoped<IForumCommentService, ForumCommentService>();

            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            builder.Services.AddScoped<ISimulationScenarioRepository, SimulationScenarioRepository>();
            builder.Services.AddScoped<ISimulationScenarioService, SimulationScenarioService>();

            builder.Services.AddScoped<IReportRepository, ReportRepository>();
            builder.Services.AddScoped<IReportService, ReportService>();

            builder.Services.AddScoped<ITrafficSignRepository, TrafficSignRepository>();
            builder.Services.AddScoped<ITrafficSignService, TrafficSignService>();

            builder.Services.AddScoped<IResolveRepository, ResolveRepository>();
            builder.Services.AddScoped<IResolveService, ResolveService>();

            builder.Services.AddScoped<ISituationExamRepository, SituationExamRepository>();
            builder.Services.AddScoped<ISituationExamService, SituationExamService>();

            builder.Services.AddScoped<ISimulationSessionRepository, SimulationSessionRepository>();
            builder.Services.AddScoped<ISimulationSessionService, SimulationSessionService>();

            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

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

            builder.Services.AddAuthorization();

            // ================= SWAGGER =================
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // 🔥 cấu hình Bearer Token
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Nhập token theo dạng: {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
<<<<<<< HEAD
            app.UseAuthentication();
=======

            // Enable CORS for requests from local frontend
            app.UseCors("LocalFrontend");

>>>>>>> 4741d5fdf77a675702d477296955ecfa7036bcec
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
