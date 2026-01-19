using Ardalis.Specification;

namespace GymDogs.Domain.Users.Specification;

public class GetUserByIdSpec : Specification<User>
{
    public GetUserByIdSpec(Guid id)
    {
        Query.Where(u => u.Id == id);
    }
}
