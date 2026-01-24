using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Users;

namespace GymDogs.Application.Users.Commands;

public record UpdateUserEmailCommand(Guid Id, string Email)
    : ICommand<Result<GetUserDto>>;

internal class UpdateUserEmailCommandHandler : ICommandHandler<UpdateUserEmailCommand, Result<GetUserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecificationFactory _specificationFactory;

    public UpdateUserEmailCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        ISpecificationFactory specificationFactory)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<GetUserDto>> Handle(
        UpdateUserEmailCommand request,
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

        var existingUserByEmail = await _userRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetUserByEmailSpec(request.Email),
            cancellationToken);

        if (existingUserByEmail != null && existingUserByEmail.Id != request.Id)
        {
            return Result<GetUserDto>.Conflict("A user with the given email already exists.");
        }

        var emailNormalized = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        user.UpdateEmail(emailNormalized);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToGetUserDto());
    }
}
