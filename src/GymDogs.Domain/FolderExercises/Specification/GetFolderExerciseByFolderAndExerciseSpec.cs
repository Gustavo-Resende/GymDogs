using Ardalis.Specification;

namespace GymDogs.Domain.FolderExercises.Specification;

public class GetFolderExerciseByFolderAndExerciseSpec : Specification<FolderExercise>
{
    public GetFolderExerciseByFolderAndExerciseSpec(Guid workoutFolderId, Guid exerciseId)
    {
        Query.Where(fe => fe.WorkoutFolderId == workoutFolderId && fe.ExerciseId == exerciseId);
    }
}
