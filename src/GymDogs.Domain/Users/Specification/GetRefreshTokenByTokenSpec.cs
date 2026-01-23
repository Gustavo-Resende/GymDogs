using Ardalis.Specification;

namespace GymDogs.Domain.Users.Specification;

public class GetRefreshTokenByTokenSpec : Specification<RefreshToken>
{
    public GetRefreshTokenByTokenSpec(string token)
    {
        Query.Where(rt => rt.Token == token);
    }
}
