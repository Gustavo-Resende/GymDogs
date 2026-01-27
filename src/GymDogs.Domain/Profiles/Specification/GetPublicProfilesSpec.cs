using Ardalis.Specification;
using GymDogs.Domain.Profiles;

namespace GymDogs.Domain.Profiles.Specification;

public class GetPublicProfilesSpec : Specification<Profile>
{
    public GetPublicProfilesSpec()
    {
        Query.Where(p => p.Visibility == ProfileVisibility.Public)
             .Include(p => p.User)
             .AsNoTracking() // Optimization: does not track entities for read-only queries
             .OrderByDescending(p => p.CreatedAt);
    }
}
