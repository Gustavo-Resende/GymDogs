using Ardalis.Result;
using GymDogs.Application.Common;

namespace GymDogs.Presentation.Services;

public class ExceptionToResultMapper : IExceptionToResultMapper
{
    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        return exception switch
        {
            ArgumentNullException nullEx => Result<T>.Invalid(
                new List<ValidationError>
                {
                    new()
                    {
                        Identifier = nullEx.ParamName ?? "Request",
                        ErrorMessage = $"{nullEx.ParamName ?? "Parameter"} cannot be null."
                    }
                }),

            ArgumentException argEx => Result<T>.Invalid(
                new List<ValidationError>
                {
                    new()
                    {
                        Identifier = argEx.ParamName ?? "Request",
                        ErrorMessage = argEx.Message
                    }
                }),

            InvalidOperationException opEx => Result<T>.Error(opEx.Message),

            _ => Result<T>.Error("An unexpected error occurred.")
        };
    }
}
