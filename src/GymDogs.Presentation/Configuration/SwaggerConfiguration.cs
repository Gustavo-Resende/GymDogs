using Microsoft.OpenApi;

namespace GymDogs.Presentation.Configuration;

/// <summary>
/// Swagger/OpenAPI configuration for GymDogs API
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Configures Swagger services
    /// </summary>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "GymDogs API",
                Version = "v1",
                Description = "REST API for workout and exercise management",
                Contact = new OpenApiContact
                {
                    Name = "GymDogs Team",
                    Email = "support@gymdogs.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License"
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
                              "Enter 'Bearer' [space] and then your token in the text box below.\r\n\r\n" +
                              "Example: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            c.OrderActionsBy(apiDesc => apiDesc.GroupName ?? apiDesc.ActionDescriptor.RouteValues["controller"] ?? string.Empty);
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger pipeline to display only in development environment
    /// </summary>
    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // Configure OpenAPI JSON endpoint
            app.MapOpenApi();

            // Configure Swagger UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/openapi/v1.json", "GymDogs API v1");
                c.RoutePrefix = "swagger";
                
                // UX settings
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.EnableFilter();
                c.ShowExtensions();
                c.EnableTryItOutByDefault();
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                
                // UI customization
                c.DefaultModelsExpandDepth(-1);
                c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
            });
        }

        return app;
    }
}
