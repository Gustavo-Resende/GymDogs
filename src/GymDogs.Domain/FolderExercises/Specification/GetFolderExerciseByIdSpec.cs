using Ardalis.Specification;

namespace GymDogs.Domain.FolderExercises.Specification;

public class GetFolderExerciseByIdSpec : Specification<FolderExercise>
{
    public GetFolderExerciseByIdSpec(Guid id)
    {
        Query.Where(fe => fe.Id == id)
             .Include(fe => fe.Exercise)
             .Include(fe => fe.WorkoutFolder)
                 .ThenInclude(wf => wf.Profile);
    }
}
