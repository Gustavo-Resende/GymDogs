using Ardalis.Specification;

namespace GymDogs.Domain.FolderExercises.Specification;

public class GetFolderExercisesByFolderIdSpec : Specification<FolderExercise>
{
    public GetFolderExercisesByFolderIdSpec(Guid workoutFolderId)
    {
        Query.Where(fe => fe.WorkoutFolderId == workoutFolderId)
             .Include(fe => fe.Exercise)
             .AsNoTracking() // Otimização: não rastreia entidades para queries de leitura
             .OrderBy(fe => fe.Order)
             .ThenBy(fe => fe.CreatedAt);
    }
}
