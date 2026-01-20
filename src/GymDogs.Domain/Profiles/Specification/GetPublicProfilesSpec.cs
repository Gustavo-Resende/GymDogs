using Ardalis.Specification;
using GymDogs.Domain.Profiles;

namespace GymDogs.Domain.Profiles.Specification;

public class GetPublicProfilesSpec : Specification<Profile>
{
    public GetPublicProfilesSpec()
    {
        Query.Where(p => p.Visibility == ProfileVisibility.Public)
             .OrderByDescending(p => p.CreatedAt);
    }
}
