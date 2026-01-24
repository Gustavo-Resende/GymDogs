using Ardalis.Result;
using Ardalis.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Commands;
using GymDogs.Application.Users.Dtos;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.Users;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.Users;

public class CreateUserCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateUser()
    {
        var userRepoMock = MockHelpers.CreateRepositoryMock<User>();
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        userRepoMock.SetupFirstOrDefaultAsync<User>(null);
        userRepoMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken ct) => u);
        profileRepoMock.Setup(x => x.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile p, CancellationToken ct) => p);

        var handler = new CreateUserCommandHandler(
            userRepoMock.Object,
            profileRepoMock.Object,
            passwordHasherMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateUserCommand("testuser", "test@example.com", "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("testuser", result.Value.Username);
        Assert.Equal("test@example.com", result.Value.Email);
        userRepoMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        profileRepoMock.Verify(x => x.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldReturnConflict()
    {
        var existingUser = new User("existinguser", "test@example.com", "hash", Role.User);
        var userRepoMock = MockHelpers.CreateRepositoryMock<User>();
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        userRepoMock.SetupFirstOrDefaultAsync(existingUser);

        var handler = new CreateUserCommandHandler(
            userRepoMock.Object,
            profileRepoMock.Object,
            passwordHasherMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateUserCommand("newuser", "test@example.com", "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Contains("email already exists", result.Errors.First(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_WithExistingUsername_ShouldReturnConflict()
    {
        var existingUser = new User("testuser", "other@example.com", "hash", Role.User);
        var userRepoMock = MockHelpers.CreateRepositoryMock<User>();
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        userRepoMock.SetupSequence(x => x.FirstOrDefaultAsync(
            It.IsAny<ISpecification<User>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null)
            .ReturnsAsync(existingUser);

        var handler = new CreateUserCommandHandler(
            userRepoMock.Object,
            profileRepoMock.Object,
            passwordHasherMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateUserCommand("testuser", "test@example.com", "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Contains("username already exists", result.Errors.First(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_WithEmailWithWhitespace_ShouldNormalizeEmail()
    {
        var userRepoMock = MockHelpers.CreateRepositoryMock<User>();
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        userRepoMock.SetupSequence(x => x.FirstOrDefaultAsync(
            It.IsAny<ISpecification<User>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null)
            .ReturnsAsync((User?)null);
        userRepoMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken ct) => u);
        profileRepoMock.Setup(x => x.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile p, CancellationToken ct) => p);

        var handler = new CreateUserCommandHandler(
            userRepoMock.Object,
            profileRepoMock.Object,
            passwordHasherMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateUserCommand("testuser", "  TEST@EXAMPLE.COM  ", "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        userRepoMock.Verify(x => x.FirstOrDefaultAsync(
            It.IsAny<ISpecification<User>>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_WithUsernameWithWhitespace_ShouldNormalizeUsername()
    {
        var userRepoMock = MockHelpers.CreateRepositoryMock<User>();
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        userRepoMock.SetupFirstOrDefaultAsync<User>(null);
        userRepoMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken ct) => u);
        profileRepoMock.Setup(x => x.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile p, CancellationToken ct) => p);

        var handler = new CreateUserCommandHandler(
            userRepoMock.Object,
            profileRepoMock.Object,
            passwordHasherMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateUserCommand("  testuser  ", "test@example.com", "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("testuser", result.Value.Username);
    }

    [Fact]
    public async Task Handle_WithVeryLongUsername_ShouldCreateUser()
    {
        var longUsername = new string('a', 1000);
        var userRepoMock = MockHelpers.CreateRepositoryMock<User>();
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        userRepoMock.SetupFirstOrDefaultAsync<User>(null);
        userRepoMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken ct) => u);
        profileRepoMock.Setup(x => x.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile p, CancellationToken ct) => p);

        var handler = new CreateUserCommandHandler(
            userRepoMock.Object,
            profileRepoMock.Object,
            passwordHasherMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateUserCommand(longUsername, "test@example.com", "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(longUsername.Trim(), result.Value.Username);
    }

    [Fact]
    public async Task Handle_WithVeryLongEmail_ShouldCreateUser()
    {
        var longEmail = new string('a', 1000) + "@example.com";
        var userRepoMock = MockHelpers.CreateRepositoryMock<User>();
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        userRepoMock.SetupFirstOrDefaultAsync<User>(null);
        userRepoMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken ct) => u);
        profileRepoMock.Setup(x => x.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile p, CancellationToken ct) => p);

        var handler = new CreateUserCommandHandler(
            userRepoMock.Object,
            profileRepoMock.Object,
            passwordHasherMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateUserCommand("testuser", longEmail, "Password123!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(longEmail.Trim().ToLowerInvariant(), result.Value.Email);
    }

    [Fact]
    public async Task Handle_WithNullEmail_ShouldThrowException()
    {
        var userRepoMock = MockHelpers.CreateRepositoryMock<User>();
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        userRepoMock.SetupSequence(x => x.FirstOrDefaultAsync(
            It.IsAny<ISpecification<User>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null)
            .ReturnsAsync((User?)null);

        var handler = new CreateUserCommandHandler(
            userRepoMock.Object,
            profileRepoMock.Object,
            passwordHasherMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateUserCommand("testuser", null!, "Password123!");

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldHashPassword()
    {
        var userRepoMock = MockHelpers.CreateRepositoryMock<User>();
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var passwordHasherMock = MockHelpers.CreatePasswordHasherMock();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        userRepoMock.SetupFirstOrDefaultAsync<User>(null);
        userRepoMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken ct) => u);
        profileRepoMock.Setup(x => x.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Profile p, CancellationToken ct) => p);

        var handler = new CreateUserCommandHandler(
            userRepoMock.Object,
            profileRepoMock.Object,
            passwordHasherMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateUserCommand("testuser", "test@example.com", "MyPassword123!");

        await handler.Handle(command, CancellationToken.None);

        passwordHasherMock.Verify(x => x.HashPassword("MyPassword123!"), Times.Once);
    }
}
