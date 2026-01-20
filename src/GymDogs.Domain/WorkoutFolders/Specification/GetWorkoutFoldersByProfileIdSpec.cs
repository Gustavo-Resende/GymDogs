using Ardalis.Specification;

namespace GymDogs.Domain.WorkoutFolders.Specification;

public class GetWorkoutFoldersByProfileIdSpec : Specification<WorkoutFolder>
{
    public GetWorkoutFoldersByProfileIdSpec(Guid profileId)
    {
        Query.Where(wf => wf.ProfileId == profileId)
             .OrderBy(wf => wf.Order)
             .ThenBy(wf => wf.CreatedAt);
    }
}
