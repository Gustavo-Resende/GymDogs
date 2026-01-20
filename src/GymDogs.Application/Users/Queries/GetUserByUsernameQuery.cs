using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Users;
using GymDogs.Domain.Users.Specification;

namespace GymDogs.Application.Users.Queries;

public record GetUserByUsernameQuery(string Username) : IQuery<Result<GetUserDto>>;

internal class GetUserByUsernameQueryHandler : IQueryHandler<GetUserByUsernameQuery, Result<GetUserDto>>
{
    private readonly IReadRepository<User> _userRepository;

    public GetUserByUsernameQueryHandler(IReadRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetUserDto>> Handle(
        GetUserByUsernameQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return Result<GetUserDto>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = "Username", ErrorMessage = "Username is required." }
                });
        }

        var usernameNormalized = request.Username.Trim();
        var user = await _userRepository.FirstOrDefaultAsync(
            new GetUserByUsernameSpec(usernameNormalized),
            cancellationToken);

        if (user == null)
        {
            return Result<GetUserDto>.NotFound($"User with username {request.Username} not found.");
        }

        return Result.Success(user.ToGetUserDto());
    }
}
