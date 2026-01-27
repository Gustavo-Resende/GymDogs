using Ardalis.Specification;

namespace GymDogs.Domain.Exercises.Specification;

/// <summary>
/// Specification to search for exercises available (not added) for a workout folder,
/// filtering by name (case-insensitive search).
/// Excludes exercises that are already in the specified folder.
/// </summary>
public class SearchAvailableExercisesForFolderSpec : Specification<Exercise>
{
    /// <summary>
    /// Initializes a new instance of the SearchAvailableExercisesForFolderSpec.
    /// </summary>
    /// <param name="workoutFolderId">The workout folder ID to exclude exercises from</param>
    /// <param name="searchTerm">The search term to filter exercises by name</param>
    public SearchAvailableExercisesForFolderSpec(Guid workoutFolderId, string searchTerm)
    {
        // Search for exercises that:
        // 1. Are NOT in the specified folder
        // 2. Name contains the search term (case-insensitive)
        var lowerSearchTerm = searchTerm.ToLowerInvariant();
        Query.Where(e => !e.FolderExercises.Any(fe => fe.WorkoutFolderId == workoutFolderId) &&
                        e.Name.ToLower().Contains(lowerSearchTerm))
             .AsNoTracking() // Optimization: does not track entities for read-only queries
             .OrderBy(e => e.Name);
    }
}
