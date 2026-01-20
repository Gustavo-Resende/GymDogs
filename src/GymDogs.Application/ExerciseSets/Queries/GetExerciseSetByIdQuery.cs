using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.ExerciseSets.Dtos;
using GymDogs.Application.ExerciseSets.Extensions;
using GymDogs.Domain.ExerciseSets;
using GymDogs.Domain.ExerciseSets.Specification;

namespace GymDogs.Application.ExerciseSets.Queries;

public record GetExerciseSetByIdQuery(Guid ExerciseSetId) : IQuery<Result<GetExerciseSetDto>>;

internal class GetExerciseSetByIdQueryHandler : IQueryHandler<GetExerciseSetByIdQuery, Result<GetExerciseSetDto>>
{
    private readonly IReadRepository<ExerciseSet> _exerciseSetRepository;

    public GetExerciseSetByIdQueryHandler(IReadRepository<ExerciseSet> exerciseSetRepository)
    {
        _exerciseSetRepository = exerciseSetRepository;
    }

    public async Task<Result<GetExerciseSetDto>> Handle(
        GetExerciseSetByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.ExerciseSetId == Guid.Empty)
        {
            return Result<GetExerciseSetDto>.NotFound("ExerciseSet ID is required.");
        }

        var exerciseSet = await _exerciseSetRepository.FirstOrDefaultAsync(
            new GetExerciseSetByIdSpec(request.ExerciseSetId),
            cancellationToken);

        if (exerciseSet == null)
        {
            return Result<GetExerciseSetDto>.NotFound($"ExerciseSet with ID {request.ExerciseSetId} not found.");
        }

        return Result.Success(exerciseSet.ToGetExerciseSetDto());
    }
}
