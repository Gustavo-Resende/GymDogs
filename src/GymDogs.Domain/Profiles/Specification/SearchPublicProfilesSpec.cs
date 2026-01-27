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
             .AsNoTracking() // Optimization: does not track entities for read-only queries
             .OrderByDescending(p => p.CreatedAt);
    }
}
