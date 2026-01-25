using Ardalis.Specification;
using GymDogs.Domain.Profiles;

namespace GymDogs.Domain.Profiles.Specification;

public class GetPublicProfilesSpec : Specification<Profile>
{
    public GetPublicProfilesSpec()
    {
        Query.Where(p => p.Visibility == ProfileVisibility.Public)
             .Include(p => p.User)
             .AsNoTracking() // Otimização: não rastreia entidades para queries de leitura
             .OrderByDescending(p => p.CreatedAt);
    }
}
