using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.Users;

namespace GymDogs.Application.Users.Commands;

public record CreateUserCommand(string Username, string Email, string Password)
    : ICommand<Result<CreateUserDto>>;

internal class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<CreateUserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Profile> _profileRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecificationFactory _specificationFactory;

    public CreateUserCommandHandler(
        IRepository<User> userRepository,
        IRepository<Profile> profileRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ISpecificationFactory specificationFactory)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<CreateUserDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUserByEmail = await _userRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetUserByEmailSpec(request.Email),
            cancellationToken);

        if (existingUserByEmail != null)
        {
            return Result<CreateUserDto>.Conflict("A user with the given email already exists.");
        }

        var existingUserByUsername = await _userRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetUserByUsernameSpec(request.Username),
            cancellationToken);

        if (existingUserByUsername != null)
        {
            return Result<CreateUserDto>.Conflict("A user with the given username already exists.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User(request.Username, request.Email, passwordHash);
        await _userRepository.AddAsync(user, cancellationToken);

        var profile = new Profile(user.Id, request.Username);
        await _profileRepository.AddAsync(profile, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToCreateUserDto());
    }
}