using Ardalis.Specification;

namespace GymDogs.Domain.ExerciseSets.Specification;

public class GetExerciseSetByIdSpec : Specification<ExerciseSet>
{
    public GetExerciseSetByIdSpec(Guid id)
    {
        Query.Where(es => es.Id == id)
             .Include(es => es.FolderExercise)
                 .ThenInclude(fe => fe.WorkoutFolder)
                     .ThenInclude(wf => wf.Profile);
    }
}
