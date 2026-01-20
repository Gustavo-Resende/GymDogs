using Ardalis.Specification;

namespace GymDogs.Domain.Users.Specification;

public class GetUserByEmailSpec : Specification<User>
{
    public GetUserByEmailSpec(string email)
    {
        Query.Where(u => u.Email == email);
    }
}
