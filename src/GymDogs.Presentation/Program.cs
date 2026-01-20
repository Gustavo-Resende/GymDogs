using GymDogs.Application;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Infrastructure.Persistence;
using GymDogs.Infrastructure.Services;
using GymDogs.Presentation.Middleware;
using GymDogs.Presentation.Services;
using Microsoft.EntityFrameworkCore;

namespace GymDogs.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

            builder.Services.AddScoped<IExceptionToResultMapper, ExceptionToResultMapper>();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/openapi/v1.json", "API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
