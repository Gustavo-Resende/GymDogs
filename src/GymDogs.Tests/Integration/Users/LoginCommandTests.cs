using Ardalis.Result;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Commands;
using GymDogs.Domain.Users;
using GymDogs.Tests.Integration.Helpers;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.Users;

public class LoginCommandTests
{
    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnLoginDto()
    {
        var user = new User("testuser", "test@example.com", "hashed_Password123!", Role.User);
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        var refreshTokenRepoMock = MockHelpers.CreateRepositoryMock<RefreshToken>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var jwtTokenGeneratorMock = MockHelpers.CreateJwtTokenGeneratorMock();
        var refreshTokenGeneratorMock = MockHelpers.CreateRefreshTokenGeneratorMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();
        var configMock = MockHelpers.CreateConfigurationMock();

        userRepoMock.SetupFirstOrDefaultAsync(user);
        refreshTokenRepoMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken rt, CancellationToken ct) => rt);

        var handler = new LoginCommandHandler(
            userRepoMock.Object,
            refreshTokenRepoMock.Object,
            passwordHasherMock.Object,
            jwtTokenGeneratorMock.Object,
            refreshTokenGeneratorMock.Object,
            unitOfWorkMock.Object,
            configMock.Object);

        var command = new LoginCommand("test@example.com", "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("testuser", result.Value.Username);
        Assert.Equal("test@example.com", result.Value.Email);
        Assert.NotNull(result.Value.Token);
        Assert.NotNull(result.Value.RefreshToken);
        passwordHasherMock.Verify(x => x.VerifyPassword("Password123!", "hashed_Password123!"), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldReturnUnauthorized()
    {
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        var refreshTokenRepoMock = MockHelpers.CreateRepositoryMock<RefreshToken>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var jwtTokenGeneratorMock = MockHelpers.CreateJwtTokenGeneratorMock();
        var refreshTokenGeneratorMock = MockHelpers.CreateRefreshTokenGeneratorMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();
        var configMock = MockHelpers.CreateConfigurationMock();

        userRepoMock.SetupFirstOrDefaultAsync<User>(null);

        var handler = new LoginCommandHandler(
            userRepoMock.Object,
            refreshTokenRepoMock.Object,
            passwordHasherMock.Object,
            jwtTokenGeneratorMock.Object,
            refreshTokenGeneratorMock.Object,
            unitOfWorkMock.Object,
            configMock.Object);

        var command = new LoginCommand("nonexistent@example.com", "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        var user = new User("testuser", "test@example.com", "hashed_WrongPassword", Role.User);
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        var refreshTokenRepoMock = MockHelpers.CreateRepositoryMock<RefreshToken>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var jwtTokenGeneratorMock = MockHelpers.CreateJwtTokenGeneratorMock();
        var refreshTokenGeneratorMock = MockHelpers.CreateRefreshTokenGeneratorMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();
        var configMock = MockHelpers.CreateConfigurationMock();

        userRepoMock.SetupFirstOrDefaultAsync(user);

        var handler = new LoginCommandHandler(
            userRepoMock.Object,
            refreshTokenRepoMock.Object,
            passwordHasherMock.Object,
            jwtTokenGeneratorMock.Object,
            refreshTokenGeneratorMock.Object,
            unitOfWorkMock.Object,
            configMock.Object);

        var command = new LoginCommand("test@example.com", "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmailWithWhitespace_ShouldNormalizeEmail()
    {
        var user = new User("testuser", "test@example.com", "hashed_Password123!", Role.User);
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        var refreshTokenRepoMock = MockHelpers.CreateRepositoryMock<RefreshToken>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var jwtTokenGeneratorMock = MockHelpers.CreateJwtTokenGeneratorMock();
        var refreshTokenGeneratorMock = MockHelpers.CreateRefreshTokenGeneratorMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();
        var configMock = MockHelpers.CreateConfigurationMock();

        userRepoMock.SetupFirstOrDefaultAsync(user);
        refreshTokenRepoMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken rt, CancellationToken ct) => rt);

        var handler = new LoginCommandHandler(
            userRepoMock.Object,
            refreshTokenRepoMock.Object,
            passwordHasherMock.Object,
            jwtTokenGeneratorMock.Object,
            refreshTokenGeneratorMock.Object,
            unitOfWorkMock.Object,
            configMock.Object);

        var command = new LoginCommand("  TEST@EXAMPLE.COM  ", "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WithNullEmail_ShouldReturnUnauthorized()
    {
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        var refreshTokenRepoMock = MockHelpers.CreateRepositoryMock<RefreshToken>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var jwtTokenGeneratorMock = MockHelpers.CreateJwtTokenGeneratorMock();
        var refreshTokenGeneratorMock = MockHelpers.CreateRefreshTokenGeneratorMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();
        var configMock = MockHelpers.CreateConfigurationMock();

        userRepoMock.SetupFirstOrDefaultAsync<User>(null);

        var handler = new LoginCommandHandler(
            userRepoMock.Object,
            refreshTokenRepoMock.Object,
            passwordHasherMock.Object,
            jwtTokenGeneratorMock.Object,
            refreshTokenGeneratorMock.Object,
            unitOfWorkMock.Object,
            configMock.Object);

        var command = new LoginCommand(null!, "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Unauthorized, result.Status);
    }

    [Fact]
    public async Task Handle_ShouldGenerateRefreshToken()
    {
        var user = new User("testuser", "test@example.com", "hashed_Password123!", Role.User);
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        var refreshTokenRepoMock = MockHelpers.CreateRepositoryMock<RefreshToken>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var jwtTokenGeneratorMock = MockHelpers.CreateJwtTokenGeneratorMock();
        var refreshTokenGeneratorMock = MockHelpers.CreateRefreshTokenGeneratorMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();
        var configMock = MockHelpers.CreateConfigurationMock();

        userRepoMock.SetupFirstOrDefaultAsync(user);
        refreshTokenRepoMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken rt, CancellationToken ct) => rt);

        var handler = new LoginCommandHandler(
            userRepoMock.Object,
            refreshTokenRepoMock.Object,
            passwordHasherMock.Object,
            jwtTokenGeneratorMock.Object,
            refreshTokenGeneratorMock.Object,
            unitOfWorkMock.Object,
            configMock.Object);

        var command = new LoginCommand("test@example.com", "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        refreshTokenGeneratorMock.Verify(x => x.GenerateRefreshToken(), Times.Once);
        refreshTokenRepoMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
