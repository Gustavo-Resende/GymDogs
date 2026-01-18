using GymDogs.Domain.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDogs.Infrastructure.Persistence.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.HasIndex(p => p.UserId)
            .IsUnique();

        builder.Property(p => p.DisplayName)
            .IsRequired()
            .HasMaxLength(200)
            .HasDefaultValue(string.Empty);

        builder.Property(p => p.Bio)
            .HasMaxLength(1000);

        builder.Property(p => p.Visibility)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.LastUpdatedAt)
            .IsRequired();

        // Relacionamento 1:1 com User
        builder.HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento 1:N com WorkoutFolders
        builder.HasMany(p => p.WorkoutFolders)
            .WithOne(wf => wf.Profile)
            .HasForeignKey(wf => wf.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
