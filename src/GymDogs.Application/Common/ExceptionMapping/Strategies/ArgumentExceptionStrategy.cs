using Ardalis.Result;

namespace GymDogs.Application.Common.ExceptionMapping.Strategies;

/// <summary>
/// Estratégia para mapear ArgumentException para Result.Invalid.
/// Trata casos onde um argumento é inválido (mas não nulo).
/// </summary>
public class ArgumentExceptionStrategy : IExceptionMappingStrategy
{
    public bool CanHandle(Exception exception)
    {
        return exception is ArgumentException && exception is not ArgumentNullException;
    }

    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        var argEx = (ArgumentException)exception;
        
        return Result<T>.Invalid(
            new List<ValidationError>
            {
                new()
                {
                    Identifier = argEx.ParamName ?? "Request",
                    ErrorMessage = argEx.Message
                }
            });
    }
}
