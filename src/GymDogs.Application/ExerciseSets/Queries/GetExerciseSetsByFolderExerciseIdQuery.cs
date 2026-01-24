using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.ExerciseSets.Dtos;
using GymDogs.Application.ExerciseSets.Extensions;
using GymDogs.Domain.ExerciseSets;

namespace GymDogs.Application.ExerciseSets.Queries;

public record GetExerciseSetsByFolderExerciseIdQuery(Guid FolderExerciseId) 
    : IQuery<Result<IEnumerable<GetExerciseSetDto>>>;

internal class GetExerciseSetsByFolderExerciseIdQueryHandler 
    : IQueryHandler<GetExerciseSetsByFolderExerciseIdQuery, Result<IEnumerable<GetExerciseSetDto>>>
{
    private readonly IReadRepository<ExerciseSet> _exerciseSetRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public GetExerciseSetsByFolderExerciseIdQueryHandler(
        IReadRepository<ExerciseSet> exerciseSetRepository,
        ISpecificationFactory specificationFactory)
    {
        _exerciseSetRepository = exerciseSetRepository;
        _specificationFactory = specificationFactory;
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
            _specificationFactory.CreateGetExerciseSetsByFolderExerciseIdSpec(request.FolderExerciseId),
            cancellationToken);

        var exerciseSetDtos = exerciseSets.Select(es => es.ToGetExerciseSetDto());

        return Result.Success(exerciseSetDtos);
    }
}
