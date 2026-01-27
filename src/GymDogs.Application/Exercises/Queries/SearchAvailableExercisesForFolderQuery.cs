using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Exercises.Dtos;
using GymDogs.Application.Exercises.Extensions;
using GymDogs.Domain.Exercises;

namespace GymDogs.Application.Exercises.Queries;

/// <summary>
/// Query to search for exercises available (not added) for a workout folder,
/// filtering by name (case-insensitive search).
/// </summary>
/// <param name="WorkoutFolderId">The unique identifier of the workout folder</param>
/// <param name="SearchTerm">The search term to filter exercises by name</param>
public record SearchAvailableExercisesForFolderQuery(Guid WorkoutFolderId, string SearchTerm) : IQuery<Result<IEnumerable<GetExerciseDto>>>;

/// <summary>
/// Handler for searching available exercises for a workout folder by name.
/// </summary>
internal class SearchAvailableExercisesForFolderQueryHandler : IQueryHandler<SearchAvailableExercisesForFolderQuery, Result<IEnumerable<GetExerciseDto>>>
{
    private readonly IReadRepository<Exercise> _exerciseRepository;
    private readonly ISpecificationFactory _specificationFactory;

    /// <summary>
    /// Initializes a new instance of the SearchAvailableExercisesForFolderQueryHandler.
    /// </summary>
    /// <param name="exerciseRepository">The exercise repository</param>
    /// <param name="specificationFactory">The specification factory</param>
    public SearchAvailableExercisesForFolderQueryHandler(
        IReadRepository<Exercise> exerciseRepository,
        ISpecificationFactory specificationFactory)
    {
        _exerciseRepository = exerciseRepository;
        _specificationFactory = specificationFactory;
    }

    /// <summary>
    /// Handles the query to search for available exercises for a workout folder by name.
    /// </summary>
    /// <param name="request">The query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A result containing the list of matching available exercises</returns>
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
