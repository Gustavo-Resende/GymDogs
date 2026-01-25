using Ardalis.Specification;

namespace GymDogs.Domain.ExerciseSets.Specification;

public class GetExerciseSetsByFolderExerciseIdSpec : Specification<ExerciseSet>
{
    public GetExerciseSetsByFolderExerciseIdSpec(Guid folderExerciseId)
    {
        Query.Where(es => es.FolderExerciseId == folderExerciseId)
             .AsNoTracking() // Otimização: não rastreia entidades para queries de leitura
             .OrderBy(es => es.SetNumber);
    }
}
