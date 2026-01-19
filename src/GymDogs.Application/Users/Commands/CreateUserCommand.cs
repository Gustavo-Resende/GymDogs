using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Extensions;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.Users;
using GymDogs.Domain.Users.Specification;
using System.Security.Cryptography;
using System.Text;

namespace GymDogs.Application.Users.Commands;

public record CreateUserCommand(string Username, string Email, string Password)
    : ICommand<Result<CreateUserDto>>;

internal class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<CreateUserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Profile> _profileRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    public CreateUserCommandHandler(
        IRepository<User> userRepository,
        IRepository<Profile> profileRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateUserDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return Result<CreateUserDto>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = "Username", ErrorMessage = "Username is required." },
                    new() { Identifier = "Email", ErrorMessage = "Email is required." },
                    new() { Identifier = "Password", ErrorMessage = "Password is required." }
                });
        }

        var emailNormalized = request.Email.Trim().ToLowerInvariant();
        var usernameNormalized = request.Username.Trim();

        var existingUserByEmail = await _userRepository.FirstOrDefaultAsync(
            new GetUserByEmailSpec(emailNormalized),
            cancellationToken);

        if (existingUserByEmail != null)
        {
            return Result<CreateUserDto>.Conflict("A user with the given email already exists.");
        }

        var existingUserByUsername = await _userRepository.FirstOrDefaultAsync(
            new GetUserByUsernameSpec(usernameNormalized),
            cancellationToken);

        if (existingUserByUsername != null)
        {
            return Result<CreateUserDto>.Conflict("A user with the given username already exists.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User(usernameNormalized, emailNormalized, passwordHash);
        await _userRepository.AddAsync(user, cancellationToken);

        var profile = new Profile(user.Id, usernameNormalized);   
        await _profileRepository.AddAsync(profile, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToCreateUserDto());
    }
}