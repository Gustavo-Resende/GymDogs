using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Domain.Users;
using GymDogs.Domain.Users.Specification;

namespace GymDogs.Application.Users.Commands;

public record LoginCommand(string Email, string Password)
    : ICommand<Result<LoginDto>>;

internal class LoginCommandHandler : ICommandHandler<LoginCommand, Result<LoginDto>>
{
    private readonly IReadRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IReadRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<LoginDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;

        var user = await _userRepository.FirstOrDefaultAsync(
            new GetUserByEmailSpec(emailNormalized),
            cancellationToken);

        if (user == null)
        {
            return Result<LoginDto>.Unauthorized("Invalid email or password.");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<LoginDto>.Unauthorized("Invalid email or password.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Username, user.Email);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        var loginDto = new LoginDto
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            ExpiresAt = expiresAt
        };

        return Result.Success(loginDto);
    }
}