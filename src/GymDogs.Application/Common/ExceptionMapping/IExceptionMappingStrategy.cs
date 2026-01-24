using Ardalis.Result;

namespace GymDogs.Application.Common.ExceptionMapping;

/// <summary>
/// Interface que define o contrato para estratégias de mapeamento de exceções.
/// Cada estratégia é responsável por tratar um tipo específico de exceção.
/// </summary>
public interface IExceptionMappingStrategy
{
    /// <summary>
    /// Verifica se esta estratégia pode tratar o tipo de exceção fornecido.
    /// </summary>
    /// <param name="exception">A exceção a ser verificada</param>
    /// <returns>True se a estratégia pode tratar a exceção, False caso contrário</returns>
    bool CanHandle(Exception exception);

    /// <summary>
    /// Mapeia a exceção para um Result apropriado.
    /// </summary>
    /// <typeparam name="T">O tipo do valor de retorno do Result</typeparam>
    /// <param name="exception">A exceção a ser mapeada</param>
    /// <returns>Um Result com o status e mensagens apropriadas</returns>
    Result<T> MapToResult<T>(Exception exception) where T : class;
}
