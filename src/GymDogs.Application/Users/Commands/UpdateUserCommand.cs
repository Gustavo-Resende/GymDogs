using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Users;

namespace GymDogs.Application.Users.Commands;

public record UpdateUserCommand(Guid Id, string? Username, string? Email)
    : ICommand<Result<GetUserDto>>;

internal class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, Result<GetUserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecificationFactory _specificationFactory;

    public UpdateUserCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        ISpecificationFactory specificationFactory)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<GetUserDto>> Handle(
        UpdateUserCommand request,
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

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            var existingUserByUsername = await _userRepository.FirstOrDefaultAsync(
                _specificationFactory.CreateGetUserByUsernameSpec(request.Username),
                cancellationToken);

            if (existingUserByUsername != null && existingUserByUsername.Id != request.Id)
            {
                return Result<GetUserDto>.Conflict("A user with the given username already exists.");
            }

            var usernameNormalized = request.Username?.Trim() ?? string.Empty;
            user.UpdateUsername(usernameNormalized);
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingUserByEmail = await _userRepository.FirstOrDefaultAsync(
                _specificationFactory.CreateGetUserByEmailSpec(request.Email),
                cancellationToken);

            if (existingUserByEmail != null && existingUserByEmail.Id != request.Id)
            {
                return Result<GetUserDto>.Conflict("A user with the given email already exists.");
            }

            var emailNormalized = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;
            user.UpdateEmail(emailNormalized);
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToGetUserDto());
    }
}
