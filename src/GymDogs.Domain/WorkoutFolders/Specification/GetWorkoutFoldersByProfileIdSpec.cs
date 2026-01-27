using Ardalis.Specification;

namespace GymDogs.Domain.WorkoutFolders.Specification;

public class GetWorkoutFoldersByProfileIdSpec : Specification<WorkoutFolder>
{
    public GetWorkoutFoldersByProfileIdSpec(Guid profileId)
    {
        Query.Where(wf => wf.ProfileId == profileId)
             .AsNoTracking() // Optimization: does not track entities for read-only queries
             .OrderBy(wf => wf.Order)
             .ThenBy(wf => wf.CreatedAt);
    }
}
