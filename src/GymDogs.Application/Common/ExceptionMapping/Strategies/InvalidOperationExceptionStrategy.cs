using Ardalis.Result;

namespace GymDogs.Application.Common.ExceptionMapping.Strategies;

/// <summary>
/// Estratégia para mapear InvalidOperationException para Result.Error.
/// Trata casos onde uma operação não pode ser executada no estado atual.
/// </summary>
public class InvalidOperationExceptionStrategy : IExceptionMappingStrategy
{
    public bool CanHandle(Exception exception)
    {
        return exception is InvalidOperationException;
    }

    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        var opEx = (InvalidOperationException)exception;
        return Result<T>.Error(opEx.Message);
    }
}
