using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Users;
using GymDogs.Domain.Users.Specification;

namespace GymDogs.Application.Users.Queries;

public record GetUserByEmailQuery(string Email) : IQuery<Result<GetUserDto>>;

internal class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, Result<GetUserDto>>
{
    private readonly IReadRepository<User> _userRepository;

    public GetUserByEmailQueryHandler(IReadRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetUserDto>> Handle(
        GetUserByEmailQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result<GetUserDto>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = "Email", ErrorMessage = "Email is required." }
                });
        }

        var emailNormalized = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.FirstOrDefaultAsync(
            new GetUserByEmailSpec(emailNormalized),
            cancellationToken);

        if (user == null)
        {
            return Result<GetUserDto>.NotFound($"User with email {request.Email} not found.");
        }

        return Result.Success(user.ToGetUserDto());
    }
}
