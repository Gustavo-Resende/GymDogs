namespace GymDogs.Application.Profiles.Dtos;

public record UpdateProfileDto(
    string? DisplayName,
    string? Bio
);

public record UpdateProfileVisibilityDto(
    ProfileVisibilityDto Visibility
);
