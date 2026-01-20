using Ardalis.Specification;

namespace GymDogs.Domain.FolderExercises.Specification;

public class GetFolderExercisesByFolderIdSpec : Specification<FolderExercise>
{
    public GetFolderExercisesByFolderIdSpec(Guid workoutFolderId)
    {
        Query.Where(fe => fe.WorkoutFolderId == workoutFolderId)
             .Include(fe => fe.Exercise)
             .OrderBy(fe => fe.Order)
             .ThenBy(fe => fe.CreatedAt);
    }
}
