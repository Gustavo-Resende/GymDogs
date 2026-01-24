using Ardalis.Result;

namespace GymDogs.Application.Common.ExceptionMapping.Strategies;

/// <summary>
/// Estratégia para mapear ArgumentNullException para Result.Invalid.
/// Trata casos onde um parâmetro obrigatório é nulo.
/// </summary>
public class ArgumentNullExceptionStrategy : IExceptionMappingStrategy
{
    public bool CanHandle(Exception exception)
    {
        return exception is ArgumentNullException;
    }

    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        var argNullEx = (ArgumentNullException)exception;
        
        return Result<T>.Invalid(
            new List<ValidationError>
            {
                new()
                {
                    Identifier = argNullEx.ParamName ?? "Request",
                    ErrorMessage = $"{argNullEx.ParamName ?? "Parameter"} cannot be null."
                }
            });
    }
}
