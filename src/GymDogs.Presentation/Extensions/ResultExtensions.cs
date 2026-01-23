using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace GymDogs.Presentation.Extensions;

/// <summary>
/// Extensions to convert Result<T> to appropriate HTTP responses
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a Result<T> to an appropriate ActionResult
    /// </summary>
    public static ActionResult<T> ToActionResult<T>(this Result<T> result) where T : class
    {
        return result.Status switch
        {
            ResultStatus.Ok => new OkObjectResult(result.Value),
            ResultStatus.Created => CreateCreatedResult(result.Value),
            ResultStatus.Invalid => new BadRequestObjectResult(new
            {
                status = "Invalid",
                errors = result.ValidationErrors
            }),
            ResultStatus.NotFound => new NotFoundObjectResult(new
            {
                status = "NotFound",
                errorMessage = result.Errors.Any() ? string.Join("; ", result.Errors) : "Resource not found"
            }),
            ResultStatus.Unauthorized => new UnauthorizedObjectResult(new
            {
                status = "Unauthorized",
                errorMessage = result.Errors.Any() ? string.Join("; ", result.Errors) : "Unauthorized"
            }),
            ResultStatus.Forbidden => new ObjectResult(new
            {
                status = "Forbidden",
                errorMessage = result.Errors.Any() ? string.Join("; ", result.Errors) : "Forbidden"
            })
            {
                StatusCode = 403
            },
            ResultStatus.Conflict => new ConflictObjectResult(new
            {
                status = "Conflict",
                errorMessage = result.Errors.Any() ? string.Join("; ", result.Errors) : "Conflict"
            }),
            _ => new ObjectResult(new
            {
                status = "Error",
                errorMessage = result.Errors.Any() ? string.Join("; ", result.Errors) : "An error occurred"
            })
            {
                StatusCode = 500
            }
        };
    }

    /// <summary>
    /// Creates an appropriate CreatedResult with route name when possible
    /// </summary>
    private static ActionResult<T> CreateCreatedResult<T>(T? value) where T : class
    {
        if (value == null)
        {
            return new CreatedResult(string.Empty, value);
        }

        // Tries to extract the Id using reflection to create CreatedAtRoute
        var idProperty = value.GetType().GetProperty("Id");
        if (idProperty != null)
        {
            var id = idProperty.GetValue(value);
            if (id != null)
            {
                return new CreatedAtActionResult(
                    actionName: null,
                    controllerName: null,
                    routeValues: new { id },
                    value: value);
            }
        }

        return new CreatedResult(string.Empty, value);
    }

    /// <summary>
    /// Converts a Result without a return value to an appropriate ActionResult
    /// </summary>
    public static ActionResult ToActionResult(this Result result)
    {
        return result.Status switch
        {
            ResultStatus.Ok => new OkResult(),
            ResultStatus.NoContent => new NoContentResult(),
            ResultStatus.Invalid => new BadRequestObjectResult(new
            {
                status = "Invalid",
                errors = result.ValidationErrors
            }),
            ResultStatus.NotFound => new NotFoundObjectResult(new
            {
                status = "NotFound",
                errorMessage = result.Errors.Any() ? string.Join("; ", result.Errors) : "Resource not found"
            }),
            ResultStatus.Unauthorized => new UnauthorizedObjectResult(new
            {
                status = "Unauthorized",
                errorMessage = result.Errors.Any() ? string.Join("; ", result.Errors) : "Unauthorized"
            }),
            ResultStatus.Forbidden => new ObjectResult(new
            {
                status = "Forbidden",
                errorMessage = result.Errors.Any() ? string.Join("; ", result.Errors) : "Forbidden"
            })
            {
                StatusCode = 403
            },
            ResultStatus.Conflict => new ConflictObjectResult(new
            {
                status = "Conflict",
                errorMessage = result.Errors.Any() ? string.Join("; ", result.Errors) : "Conflict"
            }),
            _ => new ObjectResult(new
            {
                status = "Error",
                errorMessage = result.Errors.Any() ? string.Join("; ", result.Errors) : "An error occurred"
            })
            {
                StatusCode = 500
            }
        };
    }
}
