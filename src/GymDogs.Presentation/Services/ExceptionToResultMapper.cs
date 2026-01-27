using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.ExceptionMapping;

namespace GymDogs.Presentation.Services;

/// <summary>
/// Exception mapper to Result using the Strategy pattern.
/// Delegates the handling of each exception type to specific strategies.
/// </summary>
public class ExceptionToResultMapper : IExceptionToResultMapper
{
    private readonly IEnumerable<IExceptionMappingStrategy> _strategies;

    /// <summary>
    /// Initializes the mapper with a collection of mapping strategies.
    /// </summary>
    /// <param name="strategies">Collection of exception mapping strategies</param>
    /// <exception cref="ArgumentNullException">Thrown when strategies is null</exception>
    public ExceptionToResultMapper(IEnumerable<IExceptionMappingStrategy> strategies)
    {
        _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
    }

    /// <summary>
    /// Maps an exception to a Result using the appropriate strategy.
    /// Finds the first strategy that can handle the exception type.
    /// The order of registration in the DI container matters: specific strategies first, default last.
    /// </summary>
    /// <typeparam name="T">The type of the Result return value</typeparam>
    /// <param name="exception">The exception to be mapped</param>
    /// <returns>A Result with the appropriate status and messages</returns>
    /// <exception cref="ArgumentNullException">Thrown when exception is null</exception>
    public Result<T> MapToResult<T>(Exception exception) where T : class
    {
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        // Find the first strategy that can handle this exception
        // The order of registration in the DI container matters: specific strategies first, default last
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(exception));

        // If no strategy was found (should not happen if DefaultExceptionStrategy is registered),
        // return a generic error
        return strategy?.MapToResult<T>(exception) 
               ?? Result<T>.Error("An unexpected error occurred.");
    }
}
