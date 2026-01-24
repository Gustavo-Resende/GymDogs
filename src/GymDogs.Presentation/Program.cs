using GymDogs.Application;
using GymDogs.Application.Common;
using GymDogs.Application.Common.ExceptionMapping;
using GymDogs.Application.Common.ExceptionMapping.Strategies;
using GymDogs.Application.Interfaces;
using GymDogs.Infrastructure.Persistence;
using GymDogs.Infrastructure.Services;
using GymDogs.Presentation.Configuration;
using GymDogs.Presentation.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Scalar.AspNetCore;
using System.Text;

namespace GymDogs.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Configuration of DbContext with PostgreSQL
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // MediatR Configuration
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));

            // JWT Authentication Configuration
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] 
                ?? throw new InvalidOperationException("JWT SecretKey not configured");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey))
                };
            });

            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => 
                    policy.RequireRole(RoleConstants.Admin))
                .AddPolicy("UserOnly", policy => 
                    policy.RequireRole(RoleConstants.User));

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

            builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

            // Strategy Pattern: Registro das estratégias de mapeamento de exceções
            // A ordem importa: estratégias específicas primeiro, default por último
            builder.Services.AddScoped<IExceptionMappingStrategy, ArgumentNullExceptionStrategy>();
            builder.Services.AddScoped<IExceptionMappingStrategy, ArgumentExceptionStrategy>();
            builder.Services.AddScoped<IExceptionMappingStrategy, InvalidOperationExceptionStrategy>();
            builder.Services.AddScoped<IExceptionMappingStrategy, DefaultExceptionStrategy>();

            builder.Services.AddScoped<IExceptionToResultMapper, ExceptionToResultMapper>();

            // Configuration of OpenAPI with transformer for JWT Bearer
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<JwtBearerOpenApiTransformer>();
            });

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("GymDogs API - Documentation");
                });
            }

            app.MapControllers();

            app.Run();
        }
    }
}