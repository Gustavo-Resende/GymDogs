using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Exercises.Dtos;
using GymDogs.Application.Exercises.Extensions;
using GymDogs.Domain.Exercises;
using GymDogs.Domain.Exercises.Specification;

namespace GymDogs.Application.Exercises.Queries;

public record GetExerciseByIdQuery(Guid ExerciseId) : IQuery<Result<GetExerciseDto>>;

internal class GetExerciseByIdQueryHandler : IQueryHandler<GetExerciseByIdQuery, Result<GetExerciseDto>>
{
    private readonly IReadRepository<Exercise> _exerciseRepository;

    public GetExerciseByIdQueryHandler(IReadRepository<Exercise> exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<Result<GetExerciseDto>> Handle(
        GetExerciseByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.ExerciseId == Guid.Empty)
        {
            return Result<GetExerciseDto>.NotFound("Exercise ID is required.");
        }

        var exercise = await _exerciseRepository.FirstOrDefaultAsync(
            new GetExerciseByIdSpec(request.ExerciseId),
            cancellationToken);

        if (exercise == null)
        {
            return Result<GetExerciseDto>.NotFound($"Exercise with ID {request.ExerciseId} not found.");
        }

        return Result.Success(exercise.ToGetExerciseDto());
    }
}
