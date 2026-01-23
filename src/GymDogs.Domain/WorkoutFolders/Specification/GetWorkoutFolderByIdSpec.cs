using Ardalis.Specification;

namespace GymDogs.Domain.WorkoutFolders.Specification;

public class GetWorkoutFolderByIdSpec : Specification<WorkoutFolder>
{
    public GetWorkoutFolderByIdSpec(Guid id)
    {
        Query.Where(wf => wf.Id == id)
             .Include(wf => wf.Profile);
    }
}
