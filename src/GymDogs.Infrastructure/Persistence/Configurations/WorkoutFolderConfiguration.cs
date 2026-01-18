using GymDogs.Domain.WorkoutFolders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDogs.Infrastructure.Persistence.Configurations;

public class WorkoutFolderConfiguration : IEntityTypeConfiguration<WorkoutFolder>
{
    public void Configure(EntityTypeBuilder<WorkoutFolder> builder)
    {
        builder.ToTable("WorkoutFolders");

        builder.HasKey(wf => wf.Id);

        builder.Property(wf => wf.ProfileId)
            .IsRequired();

        builder.Property(wf => wf.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(wf => wf.Description)
            .HasMaxLength(1000);

        builder.Property(wf => wf.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(wf => wf.CreatedAt)
            .IsRequired();

        builder.Property(wf => wf.LastUpdatedAt)
            .IsRequired();

        // Relacionamento N:1 com Profile
        builder.HasOne(wf => wf.Profile)
            .WithMany(p => p.WorkoutFolders)
            .HasForeignKey(wf => wf.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento 1:N com FolderExercises
        builder.HasMany(wf => wf.Exercises)
            .WithOne(fe => fe.WorkoutFolder)
            .HasForeignKey(fe => fe.WorkoutFolderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice para otimizar busca por ProfileId e Order
        builder.HasIndex(wf => new { wf.ProfileId, wf.Order });
    }
}
