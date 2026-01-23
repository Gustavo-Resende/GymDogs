using GymDogs.Domain.Profiles;
using Xunit;

namespace GymDogs.Tests.Profiles;

public class ProfileTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateProfile()
    {
        var userId = Guid.NewGuid();
        var displayName = "Test User";
        var visibility = ProfileVisibility.Public;

        var profile = new Profile(userId, displayName, visibility);

        Assert.Equal(userId, profile.UserId);
        Assert.Equal(displayName, profile.DisplayName);
        Assert.Equal(visibility, profile.Visibility);
        Assert.NotEqual(Guid.Empty, profile.Id);
        Assert.True(profile.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_WithNullDisplayName_ShouldSetEmptyString()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, null);

        Assert.Equal(string.Empty, profile.DisplayName);
    }

    [Fact]
    public void Constructor_WithDefaultVisibility_ShouldUsePublic()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test");

        Assert.Equal(ProfileVisibility.Public, profile.Visibility);
    }

    [Fact]
    public void Constructor_WithPrivateVisibility_ShouldSetPrivate()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test", ProfileVisibility.Private);

        Assert.Equal(ProfileVisibility.Private, profile.Visibility);
    }

    [Fact]
    public void Constructor_WithDefaultUserId_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new Profile(Guid.Empty, "Test"));
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldUpdateProfile()
    {
        var profile = new Profile(Guid.NewGuid(), "Old Name");
        var newDisplayName = "New Name";
        var newBio = "New Bio";
        var oldUpdatedAt = profile.LastUpdatedAt;

        Thread.Sleep(10);
        profile.UpdateProfile(newDisplayName, newBio);

        Assert.Equal(newDisplayName, profile.DisplayName);
        Assert.Equal(newBio, profile.Bio);
        Assert.True(profile.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void UpdateProfile_WithNullDisplayName_ShouldSetEmptyString()
    {
        var profile = new Profile(Guid.NewGuid(), "Old Name");
        profile.UpdateProfile(null, "Bio");

        Assert.Equal(string.Empty, profile.DisplayName);
    }

    [Fact]
    public void UpdateProfile_WithNullBio_ShouldSetNullBio()
    {
        var profile = new Profile(Guid.NewGuid(), "Name", ProfileVisibility.Public);
        profile.UpdateProfile("Name", "Bio");
        profile.UpdateProfile("Name", null);

        Assert.Null(profile.Bio);
    }

    [Fact]
    public void UpdateVisibility_WithValidVisibility_ShouldUpdateVisibility()
    {
        var profile = new Profile(Guid.NewGuid(), "Test", ProfileVisibility.Public);
        var oldUpdatedAt = profile.LastUpdatedAt;

        Thread.Sleep(10);
        profile.UpdateVisibility(ProfileVisibility.Private);

        Assert.Equal(ProfileVisibility.Private, profile.Visibility);
        Assert.True(profile.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void UpdateVisibility_WithInvalidVisibility_ShouldThrowException()
    {
        var profile = new Profile(Guid.NewGuid(), "Test");

        Assert.Throws<System.ComponentModel.InvalidEnumArgumentException>(() => profile.UpdateVisibility((ProfileVisibility)999));
    }

    [Fact]
    public void IsVisibleTo_WithPublicProfile_ShouldReturnTrue()
    {
        var profile = new Profile(Guid.NewGuid(), "Test", ProfileVisibility.Public);

        Assert.True(profile.IsVisibleTo(null));
        Assert.True(profile.IsVisibleTo(Guid.NewGuid()));
        Assert.True(profile.IsVisibleTo(profile.UserId));
    }

    [Fact]
    public void IsVisibleTo_WithPrivateProfileAndOwner_ShouldReturnTrue()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test", ProfileVisibility.Private);

        Assert.True(profile.IsVisibleTo(userId));
    }

    [Fact]
    public void IsVisibleTo_WithPrivateProfileAndOtherUser_ShouldReturnFalse()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var profile = new Profile(userId, "Test", ProfileVisibility.Private);

        Assert.False(profile.IsVisibleTo(otherUserId));
        Assert.False(profile.IsVisibleTo(null));
    }

    [Fact]
    public void IsVisibleTo_WithPrivateProfileAndEmptyGuid_ShouldReturnFalse()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test", ProfileVisibility.Private);

        Assert.False(profile.IsVisibleTo(Guid.Empty));
    }

    [Fact]
    public void UpdateProfile_WithVeryLongDisplayName_ShouldUpdate()
    {
        var profile = new Profile(Guid.NewGuid(), "Test");
        var longDisplayName = new string('a', 1000);
        profile.UpdateProfile(longDisplayName, "Bio");

        Assert.Equal(longDisplayName, profile.DisplayName);
    }

    [Fact]
    public void UpdateProfile_WithVeryLongBio_ShouldUpdate()
    {
        var profile = new Profile(Guid.NewGuid(), "Test");
        var longBio = new string('a', 5000);
        profile.UpdateProfile("Name", longBio);

        Assert.Equal(longBio, profile.Bio);
    }

    [Fact]
    public void UpdateProfile_WithEmptyStringBio_ShouldSetEmptyString()
    {
        var profile = new Profile(Guid.NewGuid(), "Test");
        profile.UpdateProfile("Name", "");

        Assert.Equal("", profile.Bio);
    }

    [Fact]
    public void UpdateProfile_WithWhitespaceBio_ShouldSetWhitespace()
    {
        var profile = new Profile(Guid.NewGuid(), "Test");
        profile.UpdateProfile("Name", "   ");

        Assert.Equal("   ", profile.Bio);
    }

    [Fact]
    public void MultipleVisibilityUpdates_ShouldUpdateTimestampEachTime()
    {
        var profile = new Profile(Guid.NewGuid(), "Test", ProfileVisibility.Public);
        var firstUpdate = profile.LastUpdatedAt;

        Thread.Sleep(10);
        profile.UpdateVisibility(ProfileVisibility.Private);
        var secondUpdate = profile.LastUpdatedAt;

        Thread.Sleep(10);
        profile.UpdateVisibility(ProfileVisibility.Public);
        var thirdUpdate = profile.LastUpdatedAt;

        Assert.True(secondUpdate > firstUpdate);
        Assert.True(thirdUpdate > secondUpdate);
    }

    [Fact]
    public void UpdateProfile_WithSameValues_ShouldUpdateTimestamp()
    {
        var profile = new Profile(Guid.NewGuid(), "Name", ProfileVisibility.Public);
        profile.UpdateProfile("Name", "Bio");
        var oldUpdatedAt = profile.LastUpdatedAt;

        Thread.Sleep(10);
        profile.UpdateProfile("Name", "Bio");

        Assert.True(profile.LastUpdatedAt > oldUpdatedAt);
    }
}
