using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Users;

namespace GymDogs.Application.Users.Queries;

public record GetAllUsersQuery : IQuery<Result<IEnumerable<GetUserDto>>>;

internal class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, Result<IEnumerable<GetUserDto>>>
{
    private readonly IReadRepository<User> _userRepository;

    public GetAllUsersQueryHandler(IReadRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<IEnumerable<GetUserDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.ListAsync(cancellationToken);
        var userDtos = users.Select(u => u.ToGetUserDto());

        return Result.Success(userDtos);
    }
}
