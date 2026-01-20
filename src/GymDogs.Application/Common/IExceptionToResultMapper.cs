using Ardalis.Result;

namespace GymDogs.Application.Common;

public interface IExceptionToResultMapper
{
    Result<T> MapToResult<T>(Exception exception) where T : class;
}
