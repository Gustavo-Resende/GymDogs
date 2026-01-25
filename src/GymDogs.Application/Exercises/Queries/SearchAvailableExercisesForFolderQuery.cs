using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Exercises.Dtos;
using GymDogs.Application.Exercises.Extensions;
using GymDogs.Domain.Exercises;

namespace GymDogs.Application.Exercises.Queries;

/// <summary>
/// Query para buscar exercícios disponíveis (não adicionados) em uma pasta de treino,
/// filtrando por nome (busca case-insensitive).
/// </summary>
public record SearchAvailableExercisesForFolderQuery(Guid WorkoutFolderId, string SearchTerm) : IQuery<Result<IEnumerable<GetExerciseDto>>>;

internal class SearchAvailableExercisesForFolderQueryHandler : IQueryHandler<SearchAvailableExercisesForFolderQuery, Result<IEnumerable<GetExerciseDto>>>
{
    private readonly IReadRepository<Exercise> _exerciseRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public SearchAvailableExercisesForFolderQueryHandler(
        IReadRepository<Exercise> exerciseRepository,
        ISpecificationFactory specificationFactory)
    {
        _exerciseRepository = exerciseRepository;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<IEnumerable<GetExerciseDto>>> Handle(
        SearchAvailableExercisesForFolderQuery request,
        CancellationToken cancellationToken)
    {
        if (request.WorkoutFolderId == Guid.Empty)
        {
            return Result<IEnumerable<GetExerciseDto>>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = nameof(request.WorkoutFolderId), ErrorMessage = "WorkoutFolder ID is required and cannot be empty." }
                });
        }

        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return Result<IEnumerable<GetExerciseDto>>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = nameof(request.SearchTerm), ErrorMessage = "Search term is required." }
                });
        }

        var exercises = await _exerciseRepository.ListAsync(
            _specificationFactory.CreateSearchAvailableExercisesForFolderSpec(request.WorkoutFolderId, request.SearchTerm),
            cancellationToken);

        var exerciseDtos = exercises.Select(e => e.ToGetExerciseDto());

        return Result.Success(exerciseDtos);
    }
}
