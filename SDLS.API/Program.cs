
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

                options.UseNpgsql(connectionString);
            });

            builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile<MappingProfile>();
            });

            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

            builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
            builder.Services.AddScoped<ILessonImageRepository, LessonImageRepository>();

            builder.Services.AddScoped<ILessonImageService, LessonImageService>();
            builder.Services.AddScoped<IStorageService, StorageService>();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Add supabase 
            var supabaseUrl = builder.Configuration["Supabase:Url"];
            var supabaseKey = builder.Configuration["Supabase:Key"];    
            builder.Services.AddScoped(_ => new Supabase.Client(supabaseUrl, supabaseKey));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
