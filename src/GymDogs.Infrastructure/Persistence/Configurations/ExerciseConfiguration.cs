using GymDogs.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDogs.Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("Exercises");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(e => e.Name);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt)
            .IsRequired();

        // Relacionamento 1:N com FolderExercises
        builder.HasMany(e => e.FolderExercises)
            .WithOne(fe => fe.Exercise)
            .HasForeignKey(fe => fe.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict); // Não deletar Exercise se tiver FolderExercises
    }
}
