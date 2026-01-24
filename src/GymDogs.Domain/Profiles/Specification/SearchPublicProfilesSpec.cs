using Ardalis.Specification;
using GymDogs.Domain.Profiles;

namespace GymDogs.Domain.Profiles.Specification;

public class SearchPublicProfilesSpec : Specification<Profile>
{
    public SearchPublicProfilesSpec(string searchTerm)
    {
        Query.Where(p => p.Visibility == ProfileVisibility.Public &&
                        (p.DisplayName.Contains(searchTerm) ||
                         p.User != null && p.User.Username.Contains(searchTerm)))
             .Include(p => p.User)
             .OrderByDescending(p => p.CreatedAt);
    }
}
