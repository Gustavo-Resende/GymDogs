using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.ExerciseSets.Dtos;
using GymDogs.Application.ExerciseSets.Extensions;
using GymDogs.Domain.ExerciseSets;
using GymDogs.Domain.ExerciseSets.Specification;

namespace GymDogs.Application.ExerciseSets.Queries;

public record GetExerciseSetsByFolderExerciseIdQuery(Guid FolderExerciseId) 
    : IQuery<Result<IEnumerable<GetExerciseSetDto>>>;

internal class GetExerciseSetsByFolderExerciseIdQueryHandler 
    : IQueryHandler<GetExerciseSetsByFolderExerciseIdQuery, Result<IEnumerable<GetExerciseSetDto>>>
{
    private readonly IReadRepository<ExerciseSet> _exerciseSetRepository;

    public GetExerciseSetsByFolderExerciseIdQueryHandler(IReadRepository<ExerciseSet> exerciseSetRepository)
    {
        _exerciseSetRepository = exerciseSetRepository;
    }

    public async Task<Result<IEnumerable<GetExerciseSetDto>>> Handle(
        GetExerciseSetsByFolderExerciseIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.FolderExerciseId == Guid.Empty)
        {
            return Result<IEnumerable<GetExerciseSetDto>>.NotFound("FolderExercise ID is required.");
        }

        var exerciseSets = await _exerciseSetRepository.ListAsync(
            new GetExerciseSetsByFolderExerciseIdSpec(request.FolderExerciseId),
            cancellationToken);

        var exerciseSetDtos = exerciseSets.Select(es => es.ToGetExerciseSetDto());

        return Result.Success(exerciseSetDtos);
    }
}
