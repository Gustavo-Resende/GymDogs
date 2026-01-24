using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Dtos;
using GymDogs.Domain.Users;
using Microsoft.Extensions.Configuration;

namespace GymDogs.Application.Users.Commands;

public record RefreshTokenCommand(string RefreshToken) : ICommand<Result<RefreshTokenDto>>;

internal class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, Result<RefreshTokenDto>>
{
    private readonly IReadRepository<User> _userRepository;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IJwtTokenBuilder _jwtTokenBuilder;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ISpecificationFactory _specificationFactory;

    public RefreshTokenCommandHandler(
        IReadRepository<User> userRepository,
        IRepository<RefreshToken> refreshTokenRepository,
        IJwtTokenBuilder jwtTokenBuilder,
        IRefreshTokenGenerator refreshTokenGenerator,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ISpecificationFactory specificationFactory)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenBuilder = jwtTokenBuilder;
        _refreshTokenGenerator = refreshTokenGenerator;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<RefreshTokenDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Result<RefreshTokenDto>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = "RefreshToken", ErrorMessage = "Refresh token is required." }
                });
        }

        // Buscar refresh token no banco
        var refreshToken = await _refreshTokenRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetRefreshTokenByTokenSpec(request.RefreshToken),
            cancellationToken);

        if (refreshToken == null || !refreshToken.IsValid())
        {
            return Result<RefreshTokenDto>.Unauthorized("Invalid or expired refresh token.");
        }

        // Buscar usuário
        var user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
        if (user == null)
        {
            return Result<RefreshTokenDto>.NotFound("User not found.");
        }

        // Revogar refresh token antigo
        refreshToken.Revoke();

        // Gerar novo access token usando Builder Pattern
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var accessTokenExpirationMinutes = int.Parse(jwtSettings["AccessTokenExpirationMinutes"] ?? "15");
        
        var newToken = _jwtTokenBuilder
            .WithUserId(user.Id)
            .WithUsername(user.Username)
            .WithEmail(user.Email)
            .WithRole(user.Role.ToString())
            .WithExpirationMinutes(accessTokenExpirationMinutes)
            .Build();
        var expiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes);

        // Gerar novo refresh token
        var refreshTokenExpirationDays = int.Parse(jwtSettings["RefreshTokenExpirationDays"] ?? "7");
        var newRefreshTokenValue = _refreshTokenGenerator.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

        var newRefreshToken = new RefreshToken(user.Id, newRefreshTokenValue, refreshTokenExpiresAt);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var refreshTokenDto = new RefreshTokenDto
        {
            Token = newToken,
            RefreshToken = newRefreshTokenValue,
            ExpiresAt = expiresAt,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };

        return Result.Success(refreshTokenDto);
    }
}
