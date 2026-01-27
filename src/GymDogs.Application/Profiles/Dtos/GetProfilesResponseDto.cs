namespace GymDogs.Application.Profiles.Dtos;

/// <summary>
/// Response DTO for profile lists with information about empty results.
/// Provides a standardized structure for profile list responses, including empty state handling.
/// </summary>
public record GetProfilesResponseDto
{
    /// <summary>
    /// List of profiles found.
    /// </summary>
    public IEnumerable<GetProfileDto> Profiles { get; init; } = Enumerable.Empty<GetProfileDto>();

    /// <summary>
    /// Indicates whether the list is empty.
    /// </summary>
    public bool IsEmpty => !Profiles.Any();

    /// <summary>
    /// Informative message when there are no registered profiles.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Total number of profiles found.
    /// </summary>
    public int TotalCount => Profiles.Count();
}
