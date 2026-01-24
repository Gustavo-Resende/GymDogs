using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Users;

namespace GymDogs.Application.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IQuery<Result<GetUserDto>>;

internal class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<GetUserDto>>
{
    private readonly IReadRepository<User> _userRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public GetUserByIdQueryHandler(
        IReadRepository<User> userRepository,
        ISpecificationFactory specificationFactory)
    {
        _userRepository = userRepository;
        _specificationFactory = specificationFactory;
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
            _specificationFactory.CreateGetUserByIdSpec(request.Id),
            cancellationToken);

        if (user == null)
        {
            return Result<GetUserDto>.NotFound($"User with ID {request.Id} not found.");
        }

        return Result.Success(user.ToGetUserDto());
    }
}
