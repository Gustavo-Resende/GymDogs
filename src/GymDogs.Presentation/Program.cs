using GymDogs.Application;
using GymDogs.Application.Common;
using GymDogs.Application.Common.ExceptionMapping;
using GymDogs.Application.Common.ExceptionMapping.Strategies;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Infrastructure.Persistence;
using GymDogs.Infrastructure.Persistence.Specification;
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

            // CORS Configuration
            // Reads allowed origins from appsettings (by environment)
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>();

            // If not configured in appsettings, uses default values for development
            if (allowedOrigins == null || allowedOrigins.Length == 0)
            {
                allowedOrigins = builder.Environment.IsDevelopment()
                    ? new[] { "http://localhost:3000", "http://localhost:5173", "http://localhost:5174" }
                    : Array.Empty<string>();
            }

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        // Development: allows configured origins + any method/header
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials()
                              .SetPreflightMaxAge(TimeSpan.FromSeconds(86400)); // Cache preflight for 24h
                    }
                    else
                    {
                        // Production: more restrictive
                        policy.WithOrigins(allowedOrigins)
                              .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS")
                              .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
                              .AllowCredentials()
                              .SetPreflightMaxAge(TimeSpan.FromSeconds(86400));
                    }
                });
            });

            builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

            // Strategy Pattern: Registration of exception mapping strategies
            // Order matters: specific strategies first, default last
            builder.Services.AddScoped<IExceptionMappingStrategy, ArgumentNullExceptionStrategy>();
            builder.Services.AddScoped<IExceptionMappingStrategy, ArgumentExceptionStrategy>();
            builder.Services.AddScoped<IExceptionMappingStrategy, InvalidOperationExceptionStrategy>();
            builder.Services.AddScoped<IExceptionMappingStrategy, DefaultExceptionStrategy>();

            builder.Services.AddScoped<IExceptionToResultMapper, ExceptionToResultMapper>();

            // Factory Pattern: Registro do Factory de Specifications
            builder.Services.AddScoped<ISpecificationFactory, SpecificationFactory>();

            // Builder Pattern: Registro do Builder de JWT Tokens
            builder.Services.AddScoped<IJwtTokenBuilder, JwtTokenBuilder>();

            // Configuration of OpenAPI with transformer for JWT Bearer
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<JwtBearerOpenApiTransformer>();
            });

            // Response Compression - Reduz tamanho das respostas HTTP
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
                options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
            });

            // Response Caching - Cache de respostas HTTP
            builder.Services.AddResponseCaching();

            var app = builder.Build();

            // CORS must come BEFORE any other middleware
            // This allows OPTIONS (preflight) requests to be processed
            app.UseCors("AllowFrontend");

            // Response Compression - Must come before other middlewares
            app.UseResponseCompression();

            // Response Caching - Must come before UseRouting/UseEndpoints
            app.UseResponseCaching();

            // HTTPS Redirection can interfere with CORS in development
            // Only use in production
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

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