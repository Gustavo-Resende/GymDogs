namespace GymDogs.Application.Profiles.Dtos;

public record GetProfileDto(
    Guid Id,
    Guid UserId,
    string DisplayName,
    string? Bio,
    ProfileVisibilityDto Visibility,
    DateTime CreatedAt,
    DateTime LastUpdatedAt
);

public enum ProfileVisibilityDto
{
    Public = 1,
    Private = 2
}
