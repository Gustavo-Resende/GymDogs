using Ardalis.Specification;

namespace GymDogs.Domain.ExerciseSets.Specification;

public class GetExerciseSetsByFolderExerciseIdSpec : Specification<ExerciseSet>
{
    public GetExerciseSetsByFolderExerciseIdSpec(Guid folderExerciseId)
    {
        Query.Where(es => es.FolderExerciseId == folderExerciseId)
             .AsNoTracking() // Optimization: does not track entities for read-only queries
             .OrderBy(es => es.SetNumber);
    }
}
