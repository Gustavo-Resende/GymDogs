using GymDogs.Domain.Users;
using Xunit;

namespace GymDogs.Tests.Users;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateUser()
    {
        var username = "testuser";
        var email = "test@example.com";
        var passwordHash = "hashedpassword";
        var role = Role.User;

        var user = new User(username, email, passwordHash, role);

        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(role, user.Role);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
        Assert.True(user.LastUpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_WithDefaultRole_ShouldUseUserRole()
    {
        var user = new User("testuser", "test@example.com", "hash");

        Assert.Equal(Role.User, user.Role);
    }

    [Fact]
    public void Constructor_WithAdminRole_ShouldSetAdminRole()
    {
        var user = new User("admin", "admin@example.com", "hash", Role.Admin);

        Assert.Equal(Role.Admin, user.Role);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceUsername_ShouldThrowException(string? username)
    {
        Assert.ThrowsAny<ArgumentException>(() => new User(username!, "test@example.com", "hash"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceEmail_ShouldThrowException(string? email)
    {
        Assert.ThrowsAny<ArgumentException>(() => new User("testuser", email!, "hash"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpacePasswordHash_ShouldThrowException(string? passwordHash)
    {
        Assert.ThrowsAny<ArgumentException>(() => new User("testuser", "test@example.com", passwordHash!));
    }

    [Fact]
    public void UpdateEmail_WithValidEmail_ShouldUpdateEmail()
    {
        var user = new User("testuser", "old@example.com", "hash");
        var newEmail = "new@example.com";
        var oldUpdatedAt = user.LastUpdatedAt;

        Thread.Sleep(10);
        user.UpdateEmail(newEmail);

        Assert.Equal(newEmail, user.Email);
        Assert.True(user.LastUpdatedAt > oldUpdatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateEmail_WithNullOrWhiteSpaceEmail_ShouldThrowException(string? email)
    {
        var user = new User("testuser", "test@example.com", "hash");

        Assert.ThrowsAny<ArgumentException>(() => user.UpdateEmail(email!));
    }

    [Fact]
    public void UpdateUsername_WithValidUsername_ShouldUpdateUsername()
    {
        var user = new User("olduser", "test@example.com", "hash");
        var newUsername = "newuser";
        var oldUpdatedAt = user.LastUpdatedAt;

        Thread.Sleep(10);
        user.UpdateUsername(newUsername);

        Assert.Equal(newUsername, user.Username);
        Assert.True(user.LastUpdatedAt > oldUpdatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateUsername_WithNullOrWhiteSpaceUsername_ShouldThrowException(string? username)
    {
        var user = new User("testuser", "test@example.com", "hash");

        Assert.ThrowsAny<ArgumentException>(() => user.UpdateUsername(username!));
    }

    [Fact]
    public void SetRole_WithValidRole_ShouldUpdateRole()
    {
        var user = new User("testuser", "test@example.com", "hash", Role.User);
        var oldUpdatedAt = user.LastUpdatedAt;

        Thread.Sleep(10);
        user.SetRole(Role.Admin);

        Assert.Equal(Role.Admin, user.Role);
        Assert.True(user.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void SetRole_WithInvalidRole_ShouldNotThrowException()
    {
        var user = new User("testuser", "test@example.com", "hash");

        user.SetRole((Role)999);
        
        Assert.Equal((Role)999, user.Role);
    }

    [Fact]
    public void UpdateEmail_WithSameEmail_ShouldUpdateTimestamp()
    {
        var user = new User("testuser", "test@example.com", "hash");
        var oldUpdatedAt = user.LastUpdatedAt;

        Thread.Sleep(10);
        user.UpdateEmail("test@example.com");

        Assert.Equal("test@example.com", user.Email);
        Assert.True(user.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void UpdateUsername_WithSameUsername_ShouldUpdateTimestamp()
    {
        var user = new User("testuser", "test@example.com", "hash");
        var oldUpdatedAt = user.LastUpdatedAt;

        Thread.Sleep(10);
        user.UpdateUsername("testuser");

        Assert.Equal("testuser", user.Username);
        Assert.True(user.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void MultipleUpdates_ShouldUpdateTimestampEachTime()
    {
        var user = new User("user1", "email1@test.com", "hash");
        var firstUpdate = user.LastUpdatedAt;

        Thread.Sleep(10);
        user.UpdateEmail("email2@test.com");
        var secondUpdate = user.LastUpdatedAt;

        Thread.Sleep(10);
        user.UpdateUsername("user2");
        var thirdUpdate = user.LastUpdatedAt;

        Assert.True(secondUpdate > firstUpdate);
        Assert.True(thirdUpdate > secondUpdate);
    }

    [Fact]
    public void Constructor_WithVeryLongUsername_ShouldCreateUser()
    {
        var longUsername = new string('a', 1000);
        var user = new User(longUsername, "test@example.com", "hash");

        Assert.Equal(longUsername, user.Username);
    }

    [Fact]
    public void Constructor_WithVeryLongEmail_ShouldCreateUser()
    {
        var longEmail = new string('a', 1000) + "@example.com";
        var user = new User("testuser", longEmail, "hash");

        Assert.Equal(longEmail, user.Email);
    }

    [Fact]
    public void Constructor_WithSpecialCharacters_ShouldCreateUser()
    {
        var username = "user_123-test@domain";
        var email = "test+tag@example.co.uk";
        var user = new User(username, email, "hash");

        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
    }
}
