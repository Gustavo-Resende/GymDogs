using GymDogs.Application;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Infrastructure.Persistence;
using GymDogs.Infrastructure.Services;
using GymDogs.Presentation.Configuration;
using GymDogs.Presentation.Middleware;
using GymDogs.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
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

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(x => x.Value?.Errors.Count > 0)
                            .SelectMany(x => x.Value!.Errors.Select(e => new
                            {
                                Identifier = x.Key,
                                ErrorMessage = e.ErrorMessage
                            }));

                        return new BadRequestObjectResult(new
                        {
                            status = "Invalid",
                            errors = errors
                        });
                    };
                });

            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerConfiguration();

            builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

            builder.Services.AddScoped<IExceptionToResultMapper, ExceptionToResultMapper>();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline
            app.UseSwaggerConfiguration();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
