using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Exercises.Dtos;
using GymDogs.Application.Exercises.Extensions;
using GymDogs.Domain.Exercises;

namespace GymDogs.Application.Exercises.Queries;

public record GetAllExercisesQuery : IQuery<Result<IEnumerable<GetExerciseDto>>>;

internal class GetAllExercisesQueryHandler : IQueryHandler<GetAllExercisesQuery, Result<IEnumerable<GetExerciseDto>>>
{
    private readonly IReadRepository<Exercise> _exerciseRepository;

    public GetAllExercisesQueryHandler(IReadRepository<Exercise> exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<Result<IEnumerable<GetExerciseDto>>> Handle(
        GetAllExercisesQuery request,
        CancellationToken cancellationToken)
    {
        var exercises = await _exerciseRepository.ListAsync(cancellationToken);
        var exerciseDtos = exercises.OrderBy(e => e.Name).Select(e => e.ToGetExerciseDto());

        return Result.Success(exerciseDtos);
    }
}
