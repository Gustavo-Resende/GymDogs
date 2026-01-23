using Ardalis.Result;
using Ardalis.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Users.Queries;
using GymDogs.Domain.Users;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.Users;

public class GetUserByUsernameQueryTests
{
    [Fact]
    public async Task Handle_WithValidUsername_ShouldReturnUser()
    {
        var user = new User("testuser", "test@example.com", "hash", Role.User);
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        userRepoMock.SetupFirstOrDefaultAsync(user);

        var handler = new GetUserByUsernameQueryHandler(userRepoMock.Object);
        var query = new GetUserByUsernameQuery("testuser");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("testuser", result.Value.Username);
        Assert.Equal("test@example.com", result.Value.Email);
    }

    [Fact]
    public async Task Handle_WithNonExistentUsername_ShouldReturnNotFound()
    {
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        userRepoMock.SetupFirstOrDefaultAsync<User>(null);

        var handler = new GetUserByUsernameQueryHandler(userRepoMock.Object);
        var query = new GetUserByUsernameQuery("nonexistent");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Contains("not found", result.Errors.First(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_WithNullOrWhiteSpaceUsername_ShouldReturnInvalid(string? username)
    {
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        var handler = new GetUserByUsernameQueryHandler(userRepoMock.Object);
        var query = new GetUserByUsernameQuery(username!);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.NotEmpty(result.ValidationErrors);
        Assert.Contains(result.ValidationErrors, e => e.Identifier == "Username");
    }

    [Fact]
    public async Task Handle_WithUsernameWithWhitespace_ShouldNormalizeUsername()
    {
        var user = new User("testuser", "test@example.com", "hash", Role.User);
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        userRepoMock.SetupFirstOrDefaultAsync(user);

        var handler = new GetUserByUsernameQueryHandler(userRepoMock.Object);
        var query = new GetUserByUsernameQuery("  testuser  ");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        userRepoMock.Verify(x => x.FirstOrDefaultAsync(
            It.IsAny<ISpecification<User>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithVeryLongUsername_ShouldSearchForUser()
    {
        var longUsername = new string('a', 1000);
        var user = new User(longUsername, "test@example.com", "hash", Role.User);
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        userRepoMock.SetupFirstOrDefaultAsync(user);

        var handler = new GetUserByUsernameQueryHandler(userRepoMock.Object);
        var query = new GetUserByUsernameQuery(longUsername);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(longUsername, result.Value.Username);
    }

    [Fact]
    public async Task Handle_WithAdminUser_ShouldReturnUserWithAdminRole()
    {
        var user = new User("admin", "admin@example.com", "hash", Role.Admin);
        var userRepoMock = MockHelpers.CreateReadRepositoryMock<User>();
        userRepoMock.SetupFirstOrDefaultAsync(user);

        var handler = new GetUserByUsernameQueryHandler(userRepoMock.Object);
        var query = new GetUserByUsernameQuery("admin");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
