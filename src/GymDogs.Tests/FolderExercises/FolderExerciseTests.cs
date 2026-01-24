using GymDogs.Domain.FolderExercises;
using Xunit;

namespace GymDogs.Tests.FolderExercises;

public class FolderExerciseTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateFolderExercise()
    {
        var workoutFolderId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var order = 1;

        var folderExercise = new FolderExercise(workoutFolderId, exerciseId, order);

        Assert.Equal(workoutFolderId, folderExercise.WorkoutFolderId);
        Assert.Equal(exerciseId, folderExercise.ExerciseId);
        Assert.Equal(order, folderExercise.Order);
        Assert.NotEqual(Guid.Empty, folderExercise.Id);
        Assert.True(folderExercise.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_WithDefaultWorkoutFolderId_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new FolderExercise(Guid.Empty, Guid.NewGuid(), 0));
    }

    [Fact]
    public void Constructor_WithDefaultExerciseId_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new FolderExercise(Guid.NewGuid(), Guid.Empty, 0));
    }

    [Fact]
    public void Constructor_WithNegativeOrder_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new FolderExercise(Guid.NewGuid(), Guid.NewGuid(), -1));
    }

    [Fact]
    public void Constructor_WithZeroOrder_ShouldCreateFolderExercise()
    {
        var folderExercise = new FolderExercise(Guid.NewGuid(), Guid.NewGuid(), 0);

        Assert.Equal(0, folderExercise.Order);
    }

    [Fact]
    public void UpdateOrder_WithValidOrder_ShouldUpdateOrder()
    {
        var folderExercise = new FolderExercise(Guid.NewGuid(), Guid.NewGuid(), 0);
        var newOrder = 5;
        var oldUpdatedAt = folderExercise.LastUpdatedAt;

        Thread.Sleep(10);
        folderExercise.UpdateOrder(newOrder);

        Assert.Equal(newOrder, folderExercise.Order);
        Assert.True(folderExercise.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void UpdateOrder_WithZeroOrder_ShouldUpdateOrder()
    {
        var folderExercise = new FolderExercise(Guid.NewGuid(), Guid.NewGuid(), 5);
        folderExercise.UpdateOrder(0);

        Assert.Equal(0, folderExercise.Order);
    }

    [Fact]
    public void UpdateOrder_WithNegativeOrder_ShouldThrowException()
    {
        var folderExercise = new FolderExercise(Guid.NewGuid(), Guid.NewGuid(), 0);
        Assert.Throws<ArgumentException>(() => folderExercise.UpdateOrder(-1));
    }

    [Fact]
    public void Constructor_WithOrderAtMaxInt_ShouldCreateFolderExercise()
    {
        var folderExercise = new FolderExercise(Guid.NewGuid(), Guid.NewGuid(), int.MaxValue);

        Assert.Equal(int.MaxValue, folderExercise.Order);
    }

    [Fact]
    public void UpdateOrder_WithOrderAtMaxInt_ShouldUpdate()
    {
        var folderExercise = new FolderExercise(Guid.NewGuid(), Guid.NewGuid(), 0);
        folderExercise.UpdateOrder(int.MaxValue);

        Assert.Equal(int.MaxValue, folderExercise.Order);
    }

    [Fact]
    public void MultipleUpdateOrders_ShouldUpdateTimestampEachTime()
    {
        var folderExercise = new FolderExercise(Guid.NewGuid(), Guid.NewGuid(), 0);
        var firstUpdate = folderExercise.LastUpdatedAt;

        Thread.Sleep(10);
        folderExercise.UpdateOrder(5);
        var secondUpdate = folderExercise.LastUpdatedAt;

        Thread.Sleep(10);
        folderExercise.UpdateOrder(10);
        var thirdUpdate = folderExercise.LastUpdatedAt;

        Assert.True(secondUpdate > firstUpdate);
        Assert.True(thirdUpdate > secondUpdate);
    }

    [Fact]
    public void UpdateOrder_WithSameValue_ShouldUpdateTimestamp()
    {
        var folderExercise = new FolderExercise(Guid.NewGuid(), Guid.NewGuid(), 5);
        var oldUpdatedAt = folderExercise.LastUpdatedAt;

        Thread.Sleep(10);
        folderExercise.UpdateOrder(5);

        Assert.True(folderExercise.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void Constructor_WithVeryLargeOrder_ShouldCreateFolderExercise()
    {
        var largeOrder = 1000000;
        var folderExercise = new FolderExercise(Guid.NewGuid(), Guid.NewGuid(), largeOrder);

        Assert.Equal(largeOrder, folderExercise.Order);
    }

    [Fact]
    public void UpdateOrder_WithVeryLargeOrder_ShouldUpdate()
    {
        var folderExercise = new FolderExercise(Guid.NewGuid(), Guid.NewGuid(), 0);
        var largeOrder = 1000000;
        folderExercise.UpdateOrder(largeOrder);

        Assert.Equal(largeOrder, folderExercise.Order);
    }
}
