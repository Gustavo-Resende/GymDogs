using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Users;

namespace GymDogs.Application.Users.Queries;

public record GetUserByUsernameQuery(string Username) : IQuery<Result<GetUserDto>>;

internal class GetUserByUsernameQueryHandler : IQueryHandler<GetUserByUsernameQuery, Result<GetUserDto>>
{
    private readonly IReadRepository<User> _userRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public GetUserByUsernameQueryHandler(
        IReadRepository<User> userRepository,
        ISpecificationFactory specificationFactory)
    {
        _userRepository = userRepository;
        _specificationFactory = specificationFactory;
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

        var user = await _userRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetUserByUsernameSpec(request.Username),
            cancellationToken);

        if (user == null)
        {
            return Result<GetUserDto>.NotFound($"User with username {request.Username} not found.");
        }

        return Result.Success(user.ToGetUserDto());
    }
}
