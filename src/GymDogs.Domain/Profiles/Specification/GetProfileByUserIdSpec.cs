using Ardalis.Specification;

namespace GymDogs.Domain.Profiles.Specification;

public class GetProfileByUserIdSpec : Specification<Profile>
{
    public GetProfileByUserIdSpec(Guid userId)
    {
        Query.Where(p => p.UserId == userId);
    }
}
