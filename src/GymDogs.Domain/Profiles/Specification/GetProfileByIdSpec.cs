using Ardalis.Specification;

namespace GymDogs.Domain.Profiles.Specification;

public class GetProfileByIdSpec : Specification<Profile>
{
    public GetProfileByIdSpec(Guid id)
    {
        Query.Where(p => p.Id == id);
    }
}
