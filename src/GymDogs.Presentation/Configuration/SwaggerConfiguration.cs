namespace GymDogs.Presentation.Configuration;

/// <summary>
/// Configuração do Swagger/OpenAPI
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Configura os serviços do Swagger
    /// </summary>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen();

        return services;
    }

    /// <summary>
    /// Configura o pipeline do Swagger
    /// </summary>
    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/openapi/v1.json", "GymDogs API v1");
                c.RoutePrefix = "swagger";
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.EnableFilter();
                c.ShowExtensions();
            });
        }

        return app;
    }
}
