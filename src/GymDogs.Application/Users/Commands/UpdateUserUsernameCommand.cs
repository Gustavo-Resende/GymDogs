using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Users;
using GymDogs.Domain.Users.Specification;

namespace GymDogs.Application.Users.Commands;

public record UpdateUserUsernameCommand(Guid Id, string Username)
    : ICommand<Result<GetUserDto>>;

internal class UpdateUserUsernameCommandHandler : ICommandHandler<UpdateUserUsernameCommand, Result<GetUserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserUsernameCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetUserDto>> Handle(
        UpdateUserUsernameCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return Result<GetUserDto>.NotFound("User ID is required.");
        }

        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            return Result<GetUserDto>.NotFound($"User with ID {request.Id} not found.");
        }

        var usernameNormalized = request.Username?.Trim() ?? string.Empty;
        var existingUserByUsername = await _userRepository.FirstOrDefaultAsync(
            new GetUserByUsernameSpec(usernameNormalized),
            cancellationToken);

        if (existingUserByUsername != null && existingUserByUsername.Id != request.Id)
        {
            return Result<GetUserDto>.Conflict("A user with the given username already exists.");
        }

        user.UpdateUsername(usernameNormalized);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToGetUserDto());
    }
}
