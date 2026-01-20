using Ardalis.Result;
using GymDogs.Application.Common;
using System.Net;
using System.Text.Json;

namespace GymDogs.Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IExceptionToResultMapper exceptionMapper)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex, exceptionMapper);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        IExceptionToResultMapper exceptionMapper)
    {
        context.Response.ContentType = "application/json";

        var result = exceptionMapper.MapToResult<object>(exception);

        context.Response.StatusCode = result.Status switch
        {
            ResultStatus.Invalid => (int)HttpStatusCode.BadRequest,
            ResultStatus.NotFound => (int)HttpStatusCode.NotFound,
            ResultStatus.Unauthorized => (int)HttpStatusCode.Unauthorized,
            ResultStatus.Forbidden => (int)HttpStatusCode.Forbidden,
            ResultStatus.Conflict => (int)HttpStatusCode.Conflict,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            status = result.Status.ToString(),
            errors = result.ValidationErrors,
            errorMessage = result.Errors.Any() ? string.Join("; ", result.Errors) : null
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
