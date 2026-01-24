using Ardalis.Result;

namespace GymDogs.Application.Common.ExceptionMapping.Strategies;

/// <summary>
/// Estratégia padrão para mapear exceções não tratadas por outras estratégias.
/// Esta é a estratégia de fallback que sempre pode tratar qualquer exceção.
/// </summary>
public class DefaultExceptionStrategy : IExceptionMappingStrategy
{
    public bool CanHandle(Exception exception)
    {
        // Esta estratégia sempre pode tratar qualquer exceção (fallback)
        return true;
    }

    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        // Para exceções não mapeadas, retorna um erro genérico
        return Result<T>.Error("An unexpected error occurred.");
    }
}
