using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;

namespace GymDogs.Presentation.Extensions;

/// <summary>
/// Extensões para converter Result<T> em respostas HTTP apropriadas
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converte um Result<T> em uma ActionResult apropriada
    /// </summary>
    public static ActionResult<T> ToActionResult<T>(this Result<T> result) where T : class
    {
        return result.Status switch
        {
            ResultStatus.Ok => new OkObjectResult(result.Value),
            ResultStatus.Created => new CreatedResult(string.Empty, result.Value),
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
    /// Converte um Result sem valor de retorno em uma ActionResult apropriada
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
