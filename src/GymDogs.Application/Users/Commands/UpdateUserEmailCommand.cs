using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Users;
using GymDogs.Domain.Users.Specification;

namespace GymDogs.Application.Users.Commands;

public record UpdateUserEmailCommand(Guid Id, string Email)
    : ICommand<Result<GetUserDto>>;

internal class UpdateUserEmailCommandHandler : ICommandHandler<UpdateUserEmailCommand, Result<GetUserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserEmailCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetUserDto>> Handle(
        UpdateUserEmailCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return Result<GetUserDto>.NotFound("User ID is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result<GetUserDto>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = "Email", ErrorMessage = "Email is required." }
                });
        }

        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            return Result<GetUserDto>.NotFound($"User with ID {request.Id} not found.");
        }

        var emailNormalized = request.Email.Trim().ToLowerInvariant();
        var existingUserByEmail = await _userRepository.FirstOrDefaultAsync(
            new GetUserByEmailSpec(emailNormalized),
            cancellationToken);

        if (existingUserByEmail != null && existingUserByEmail.Id != request.Id)
        {
            return Result<GetUserDto>.Conflict("A user with the given email already exists.");
        }

        user.UpdateEmail(emailNormalized);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToGetUserDto());
    }
}
