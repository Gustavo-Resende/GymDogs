using GymDogs.Domain.ExerciseSets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDogs.Infrastructure.Persistence.Configurations;

public class ExerciseSetConfiguration : IEntityTypeConfiguration<ExerciseSet>
{
    public void Configure(EntityTypeBuilder<ExerciseSet> builder)
    {
        builder.ToTable("ExerciseSets");

        builder.HasKey(es => es.Id);

        builder.Property(es => es.FolderExerciseId)
            .IsRequired();

        builder.Property(es => es.SetNumber)
            .IsRequired();

        builder.Property(es => es.Reps)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(es => es.Weight)
            .IsRequired()
            .HasPrecision(10, 2)
            .HasDefaultValue(0m);

        builder.ToTable(t => t.HasCheckConstraint("CK_ExerciseSets_Reps_Range", 
            "\"Reps\" >= 1 AND \"Reps\" <= 1000"));

        builder.ToTable(t => t.HasCheckConstraint("CK_ExerciseSets_Weight_Range", 
            "\"Weight\" >= 0 AND \"Weight\" <= 10000"));

        builder.Property(es => es.CreatedAt)
            .IsRequired();

        builder.Property(es => es.LastUpdatedAt)
            .IsRequired();

        // Relacionamento N:1 com FolderExercise
        builder.HasOne(es => es.FolderExercise)
            .WithMany(fe => fe.Sets)
            .HasForeignKey(es => es.FolderExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice para otimizar busca por FolderExerciseId e SetNumber
        builder.HasIndex(es => new { es.FolderExerciseId, es.SetNumber });
    }
}
