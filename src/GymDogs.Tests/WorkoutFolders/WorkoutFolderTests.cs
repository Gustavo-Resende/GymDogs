using GymDogs.Domain.WorkoutFolders;
using Xunit;

namespace GymDogs.Tests.WorkoutFolders;

public class WorkoutFolderTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateWorkoutFolder()
    {
        var profileId = Guid.NewGuid();
        var name = "Back Workout";
        var description = "Back training";
        var order = 1;

        var folder = new WorkoutFolder(profileId, name, description, order);

        Assert.Equal(profileId, folder.ProfileId);
        Assert.Equal(name, folder.Name);
        Assert.Equal(description, folder.Description);
        Assert.Equal(order, folder.Order);
        Assert.NotEqual(Guid.Empty, folder.Id);
        Assert.True(folder.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_WithDefaultOrder_ShouldUseZero()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test");

        Assert.Equal(0, folder.Order);
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldSetNullDescription()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test", null);

        Assert.Null(folder.Description);
    }

    [Fact]
    public void Constructor_WithDefaultProfileId_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new WorkoutFolder(Guid.Empty, "Test"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceName_ShouldThrowException(string? name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new WorkoutFolder(Guid.NewGuid(), name!));
    }

    [Fact]
    public void Constructor_WithNameTooLong_ShouldThrowException()
    {
        var longName = new string('a', 201);
        Assert.Throws<ArgumentException>(() => new WorkoutFolder(Guid.NewGuid(), longName));
    }

    [Fact]
    public void Constructor_WithMaxLengthName_ShouldCreateWorkoutFolder()
    {
        var maxName = new string('a', 200);
        var folder = new WorkoutFolder(Guid.NewGuid(), maxName);

        Assert.Equal(maxName, folder.Name);
    }

    [Fact]
    public void Constructor_WithNegativeOrder_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new WorkoutFolder(Guid.NewGuid(), "Test", null, -1));
    }

    [Fact]
    public void Constructor_WithZeroOrder_ShouldCreateWorkoutFolder()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test", null, 0);

        Assert.Equal(0, folder.Order);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateWorkoutFolder()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Old Name", "Old Description");
        var newName = "New Name";
        var newDescription = "New Description";
        var oldUpdatedAt = folder.LastUpdatedAt;

        Thread.Sleep(10);
        folder.Update(newName, newDescription);

        Assert.Equal(newName, folder.Name);
        Assert.Equal(newDescription, folder.Description);
        Assert.True(folder.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void Update_WithNullDescription_ShouldSetNullDescription()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test", "Description");
        folder.Update("Test", null);

        Assert.Null(folder.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithNullOrWhiteSpaceName_ShouldThrowException(string? name)
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test");
        Assert.ThrowsAny<ArgumentException>(() => folder.Update(name!, "Description"));
    }

    [Fact]
    public void Update_WithNameTooLong_ShouldThrowException()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test");
        var longName = new string('a', 201);
        Assert.Throws<ArgumentException>(() => folder.Update(longName, "Description"));
    }

    [Fact]
    public void UpdateOrder_WithValidOrder_ShouldUpdateOrder()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test", null, 0);
        var newOrder = 5;
        var oldUpdatedAt = folder.LastUpdatedAt;

        Thread.Sleep(10);
        folder.UpdateOrder(newOrder);

        Assert.Equal(newOrder, folder.Order);
        Assert.True(folder.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void UpdateOrder_WithZeroOrder_ShouldUpdateOrder()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test", null, 5);
        folder.UpdateOrder(0);

        Assert.Equal(0, folder.Order);
    }

    [Fact]
    public void UpdateOrder_WithNegativeOrder_ShouldThrowException()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test");
        Assert.Throws<ArgumentException>(() => folder.UpdateOrder(-1));
    }

    [Fact]
    public void Constructor_WithSingleCharacterName_ShouldCreateWorkoutFolder()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "A");

        Assert.Equal("A", folder.Name);
    }

    [Fact]
    public void Update_WithSingleCharacterName_ShouldUpdate()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test");
        folder.Update("X", null);

        Assert.Equal("X", folder.Name);
    }

    [Fact]
    public void Constructor_WithOrderAtMaxInt_ShouldCreateWorkoutFolder()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test", null, int.MaxValue);

        Assert.Equal(int.MaxValue, folder.Order);
    }

    [Fact]
    public void UpdateOrder_WithOrderAtMaxInt_ShouldUpdate()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test");
        folder.UpdateOrder(int.MaxValue);

        Assert.Equal(int.MaxValue, folder.Order);
    }

    [Fact]
    public void Constructor_WithVeryLongDescription_ShouldCreateWorkoutFolder()
    {
        var longDescription = new string('a', 5000);
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test", longDescription);

        Assert.Equal(longDescription, folder.Description);
    }

    [Fact]
    public void Update_WithVeryLongDescription_ShouldUpdate()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test");
        var longDescription = new string('a', 5000);
        folder.Update("Test", longDescription);

        Assert.Equal(longDescription, folder.Description);
    }

    [Fact]
    public void Update_WithWhitespaceOnlyDescription_ShouldSetWhitespace()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test", "Description");
        folder.Update("Test", "   ");

        Assert.Equal("   ", folder.Description);
    }

    [Fact]
    public void Update_WithEmptyStringDescription_ShouldSetEmptyString()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test", "Description");
        folder.Update("Test", "");

        Assert.Equal("", folder.Description);
    }

    [Fact]
    public void MultipleUpdates_ShouldUpdateTimestampEachTime()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Name1", "Desc1");
        var firstUpdate = folder.LastUpdatedAt;

        Thread.Sleep(10);
        folder.Update("Name2", "Desc2");
        var secondUpdate = folder.LastUpdatedAt;

        Thread.Sleep(10);
        folder.UpdateOrder(10);
        var thirdUpdate = folder.LastUpdatedAt;

        Assert.True(secondUpdate > firstUpdate);
        Assert.True(thirdUpdate > secondUpdate);
    }

    [Fact]
    public void Update_WithSameValues_ShouldUpdateTimestamp()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test", "Description");
        var oldUpdatedAt = folder.LastUpdatedAt;

        Thread.Sleep(10);
        folder.Update("Test", "Description");

        Assert.True(folder.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void UpdateOrder_WithSameValue_ShouldUpdateTimestamp()
    {
        var folder = new WorkoutFolder(Guid.NewGuid(), "Test", null, 5);
        var oldUpdatedAt = folder.LastUpdatedAt;

        Thread.Sleep(10);
        folder.UpdateOrder(5);

        Assert.True(folder.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void Constructor_WithUnicodeCharacters_ShouldCreateWorkoutFolder()
    {
        var name = "Treino de Costas 💪";
        var description = "Descrição com emojis: 🏋️🔥";
        var folder = new WorkoutFolder(Guid.NewGuid(), name, description);

        Assert.Equal(name, folder.Name);
        Assert.Equal(description, folder.Description);
    }
}
