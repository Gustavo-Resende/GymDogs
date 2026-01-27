using Ardalis.Specification;

namespace GymDogs.Domain.FolderExercises.Specification;

public class GetFolderExercisesByFolderIdSpec : Specification<FolderExercise>
{
    public GetFolderExercisesByFolderIdSpec(Guid workoutFolderId)
    {
        Query.Where(fe => fe.WorkoutFolderId == workoutFolderId)
             .Include(fe => fe.Exercise)
             .AsNoTracking() // Optimization: does not track entities for read-only queries
             .OrderBy(fe => fe.Order)
             .ThenBy(fe => fe.CreatedAt);
    }
}
