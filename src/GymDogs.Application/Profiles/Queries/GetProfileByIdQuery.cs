using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.Profiles.Specification;

namespace GymDogs.Application.Profiles.Queries;

public record GetProfileByIdQuery(Guid ProfileId, Guid? CurrentUserId = null) : IQuery<Result<GetProfileDto>>;

internal class GetProfileByIdQueryHandler : IQueryHandler<GetProfileByIdQuery, Result<GetProfileDto>>
{
    private readonly IReadRepository<Profile> _profileRepository;

    public GetProfileByIdQueryHandler(IReadRepository<Profile> profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<Result<GetProfileDto>> Handle(
        GetProfileByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.ProfileId == Guid.Empty)
        {
            return Result<GetProfileDto>.NotFound("Profile ID is required.");
        }

        var profile = await _profileRepository.FirstOrDefaultAsync(
            new GetProfileByIdSpec(request.ProfileId),
            cancellationToken);

        if (profile == null)
        {
            return Result<GetProfileDto>.NotFound($"Profile with ID {request.ProfileId} not found.");
        }

        if (!profile.IsVisibleTo(request.CurrentUserId))
        {
            return Result<GetProfileDto>.Forbidden("You do not have permission to view this profile.");
        }

        return Result.Success(profile.ToGetProfileDto());
    }
}
