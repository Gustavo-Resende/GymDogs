using GymDogs.Application.Profiles.Dtos;
using GymDogs.Domain.Profiles;

namespace GymDogs.Application.Profiles.Extensions;

public static class ProfileExtensions
{
    public static GetProfileDto ToGetProfileDto(this Profile profile)
    {
        return new GetProfileDto(
            profile.Id,
            profile.UserId,
            profile.DisplayName,
            profile.Bio,
            profile.Visibility.ToProfileVisibilityDto(),
            profile.CreatedAt,
            profile.LastUpdatedAt);
    }

    private static ProfileVisibilityDto ToProfileVisibilityDto(this ProfileVisibility visibility)
    {
        return visibility switch
        {
            ProfileVisibility.Public => ProfileVisibilityDto.Public,
            ProfileVisibility.Private => ProfileVisibilityDto.Private,
            _ => ProfileVisibilityDto.Public
        };
    }

    public static ProfileVisibility ToProfileVisibility(this ProfileVisibilityDto visibilityDto)
    {
        return visibilityDto switch
        {
            ProfileVisibilityDto.Public => ProfileVisibility.Public,
            ProfileVisibilityDto.Private => ProfileVisibility.Private,
            _ => ProfileVisibility.Public
        };
    }
}
