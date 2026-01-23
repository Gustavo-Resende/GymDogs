using GymDogs.Domain.Exercises;
using GymDogs.Domain.ExerciseSets;
using GymDogs.Domain.FolderExercises;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.Users;
using GymDogs.Domain.WorkoutFolders;
using Microsoft.EntityFrameworkCore;

namespace GymDogs.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<WorkoutFolder> WorkoutFolders => Set<WorkoutFolder>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<FolderExercise> FolderExercises => Set<FolderExercise>();
    public DbSet<ExerciseSet> ExerciseSets => Set<ExerciseSet>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
