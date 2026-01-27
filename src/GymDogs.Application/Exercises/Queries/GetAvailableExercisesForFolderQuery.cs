using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Exercises.Dtos;
using GymDogs.Application.Exercises.Extensions;
using GymDogs.Domain.Exercises;

namespace GymDogs.Application.Exercises.Queries;

/// <summary>
/// Query to retrieve exercises available (not added) for a workout folder.
/// Returns all exercises that are not yet associated with the specified workout folder.
/// </summary>
/// <param name="WorkoutFolderId">The unique identifier of the workout folder</param>
public record GetAvailableExercisesForFolderQuery(Guid WorkoutFolderId) : IQuery<Result<IEnumerable<GetExerciseDto>>>;

/// <summary>
/// Handler for retrieving available exercises for a workout folder.
/// </summary>
internal class GetAvailableExercisesForFolderQueryHandler : IQueryHandler<GetAvailableExercisesForFolderQuery, Result<IEnumerable<GetExerciseDto>>>
{
    private readonly IReadRepository<Exercise> _exerciseRepository;
    private readonly ISpecificationFactory _specificationFactory;

    /// <summary>
    /// Initializes a new instance of the GetAvailableExercisesForFolderQueryHandler.
    /// </summary>
    /// <param name="exerciseRepository">The exercise repository</param>
    /// <param name="specificationFactory">The specification factory</param>
    public GetAvailableExercisesForFolderQueryHandler(
        IReadRepository<Exercise> exerciseRepository,
        ISpecificationFactory specificationFactory)
    {
        _exerciseRepository = exerciseRepository;
        _specificationFactory = specificationFactory;
    }

    /// <summary>
    /// Handles the query to retrieve available exercises for a workout folder.
    /// </summary>
    /// <param name="request">The query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A result containing the list of available exercises</returns>
    public async Task<Result<IEnumerable<GetExerciseDto>>> Handle(
        GetAvailableExercisesForFolderQuery request,
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

        var exercises = await _exerciseRepository.ListAsync(
            _specificationFactory.CreateGetAvailableExercisesForFolderSpec(request.WorkoutFolderId),
            cancellationToken);

        var exerciseDtos = exercises.Select(e => e.ToGetExerciseDto());

        return Result.Success(exerciseDtos);
    }
}
