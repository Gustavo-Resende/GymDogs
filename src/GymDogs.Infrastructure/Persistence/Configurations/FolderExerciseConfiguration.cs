using GymDogs.Domain.FolderExercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDogs.Infrastructure.Persistence.Configurations;

public class FolderExerciseConfiguration : IEntityTypeConfiguration<FolderExercise>
{
    public void Configure(EntityTypeBuilder<FolderExercise> builder)
    {
        builder.ToTable("FolderExercises");

        builder.HasKey(fe => fe.Id);

        builder.Property(fe => fe.WorkoutFolderId)
            .IsRequired();

        builder.Property(fe => fe.ExerciseId)
            .IsRequired();

        builder.Property(fe => fe.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(fe => fe.CreatedAt)
            .IsRequired();

        builder.Property(fe => fe.LastUpdatedAt)
            .IsRequired();

        // Relacionamento N:1 com WorkoutFolder
        builder.HasOne(fe => fe.WorkoutFolder)
            .WithMany(wf => wf.Exercises)
            .HasForeignKey(fe => fe.WorkoutFolderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento N:1 com Exercise
        builder.HasOne(fe => fe.Exercise)
            .WithMany(e => e.FolderExercises)
            .HasForeignKey(fe => fe.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento 1:N com ExerciseSets
        builder.HasMany(fe => fe.Sets)
            .WithOne(es => es.FolderExercise)
            .HasForeignKey(es => es.FolderExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice composto para otimizar busca e ordenação
        builder.HasIndex(fe => new { fe.WorkoutFolderId, fe.Order });

        // Evitar duplicação de Exercise na mesma pasta
        builder.HasIndex(fe => new { fe.WorkoutFolderId, fe.ExerciseId })
            .IsUnique();
    }
}
