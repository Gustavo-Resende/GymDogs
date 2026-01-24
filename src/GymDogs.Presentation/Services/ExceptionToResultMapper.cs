using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.ExceptionMapping;

namespace GymDogs.Presentation.Services;

/// <summary>
/// Mapeador de exceções para Result usando o padrão Strategy.
/// Delega o tratamento de cada tipo de exceção para estratégias específicas.
/// </summary>
public class ExceptionToResultMapper : IExceptionToResultMapper
{
    private readonly IEnumerable<IExceptionMappingStrategy> _strategies;

    /// <summary>
    /// Inicializa o mapeador com uma coleção de estratégias de mapeamento.
    /// </summary>
    /// <param name="strategies">Coleção de estratégias de mapeamento de exceções</param>
    public ExceptionToResultMapper(IEnumerable<IExceptionMappingStrategy> strategies)
    {
        _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
    }

    /// <summary>
    /// Mapeia uma exceção para um Result usando a estratégia apropriada.
    /// </summary>
    /// <typeparam name="T">O tipo do valor de retorno do Result</typeparam>
    /// <param name="exception">A exceção a ser mapeada</param>
    /// <returns>Um Result com o status e mensagens apropriadas</returns>
    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        // Encontra a primeira estratégia que pode tratar esta exceção
        // A ordem de registro no DI container importa: estratégias específicas primeiro, default por último
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(exception));

        // Se nenhuma estratégia foi encontrada (não deveria acontecer se DefaultExceptionStrategy estiver registrada),
        // retorna um erro genérico
        return strategy?.MapToResult<T>(exception) 
               ?? Result<T>.Error("An unexpected error occurred.");
    }
}
