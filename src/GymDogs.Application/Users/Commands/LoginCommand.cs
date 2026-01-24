using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Domain.Users;
using Microsoft.Extensions.Configuration;

namespace GymDogs.Application.Users.Commands;

public record LoginCommand(string Email, string Password)
    : ICommand<Result<LoginDto>>;

internal class LoginCommandHandler : ICommandHandler<LoginCommand, Result<LoginDto>>
{
    private readonly IReadRepository<User> _userRepository;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenBuilder _jwtTokenBuilder;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ISpecificationFactory _specificationFactory;

    public LoginCommandHandler(
        IReadRepository<User> userRepository,
        IRepository<RefreshToken> refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenBuilder jwtTokenBuilder,
        IRefreshTokenGenerator refreshTokenGenerator,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ISpecificationFactory specificationFactory)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenBuilder = jwtTokenBuilder;
        _refreshTokenGenerator = refreshTokenGenerator;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<LoginDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetUserByEmailSpec(request.Email),
            cancellationToken);

        if (user == null)
        {
            return Result<LoginDto>.Unauthorized("Invalid email or password.");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<LoginDto>.Unauthorized("Invalid email or password.");
        }

        // Gerar access token usando Builder Pattern (expira em minutos configurados)
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var accessTokenExpirationMinutes = int.Parse(jwtSettings["AccessTokenExpirationMinutes"] ?? "15");
        
        var token = _jwtTokenBuilder
            .WithUserId(user.Id)
            .WithUsername(user.Username)
            .WithEmail(user.Email)
            .WithRole(user.Role.ToString())
            .WithExpirationMinutes(accessTokenExpirationMinutes)
            .Build();
        var expiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes);

        // Gerar refresh token (expira em dias configurados)
        var refreshTokenExpirationDays = int.Parse(jwtSettings["RefreshTokenExpirationDays"] ?? "7");
        var refreshTokenValue = _refreshTokenGenerator.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

        // Salvar refresh token no banco
        var refreshToken = new RefreshToken(user.Id, refreshTokenValue, refreshTokenExpiresAt);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var loginDto = new LoginDto
        {
            Token = token,
            RefreshToken = refreshTokenValue,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            ExpiresAt = expiresAt,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            Role = user.Role.ToString()
        };

        return Result.Success(loginDto);
    }
}