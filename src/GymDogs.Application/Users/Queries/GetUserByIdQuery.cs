using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Users;
using GymDogs.Domain.Users.Specification;

namespace GymDogs.Application.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IQuery<Result<GetUserDto>>;

internal class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<GetUserDto>>
{
    private readonly IReadRepository<User> _userRepository;

    public GetUserByIdQueryHandler(IReadRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetUserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return Result<GetUserDto>.NotFound("User ID is required.");
        }

        var user = await _userRepository.FirstOrDefaultAsync(
            new GetUserByIdSpec(request.Id),
            cancellationToken);

        if (user == null)
        {
            return Result<GetUserDto>.NotFound($"User with ID {request.Id} not found.");
        }

        return Result.Success(user.ToGetUserDto());
    }
}
