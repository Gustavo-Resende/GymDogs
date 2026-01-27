using Ardalis.Specification;

namespace GymDogs.Domain.Exercises.Specification;

/// <summary>
/// Specification to retrieve exercises available (not added) for a workout folder.
/// Excludes exercises that are already in the specified folder.
/// </summary>
public class GetAvailableExercisesForFolderSpec : Specification<Exercise>
{
    /// <summary>
    /// Initializes a new instance of the GetAvailableExercisesForFolderSpec.
    /// </summary>
    /// <param name="workoutFolderId">The workout folder ID to exclude exercises from</param>
    public GetAvailableExercisesForFolderSpec(Guid workoutFolderId)
    {
        // Search for exercises that are NOT in the specified folder
        // Uses subquery to check if the exercise is already in the folder
        Query.Where(e => !e.FolderExercises.Any(fe => fe.WorkoutFolderId == workoutFolderId))
             .AsNoTracking() // Optimization: does not track entities for read-only queries
             .OrderBy(e => e.Name);
    }
}
