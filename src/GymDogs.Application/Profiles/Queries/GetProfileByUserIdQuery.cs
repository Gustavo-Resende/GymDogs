using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.Profiles.Specification;

namespace GymDogs.Application.Profiles.Queries;

public record GetProfileByUserIdQuery(Guid UserId) : IQuery<Result<GetProfileDto>>;

internal class GetProfileByUserIdQueryHandler : IQueryHandler<GetProfileByUserIdQuery, Result<GetProfileDto>>
{
    private readonly IReadRepository<Profile> _profileRepository;

    public GetProfileByUserIdQueryHandler(IReadRepository<Profile> profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<Result<GetProfileDto>> Handle(
        GetProfileByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result<GetProfileDto>.NotFound("User ID is required.");
        }

        var profile = await _profileRepository.FirstOrDefaultAsync(
            new GetProfileByUserIdSpec(request.UserId),
            cancellationToken);

        if (profile == null)
        {
            return Result<GetProfileDto>.NotFound($"Profile for User ID {request.UserId} not found.");
        }

        return Result.Success(profile.ToGetProfileDto());
    }
}
