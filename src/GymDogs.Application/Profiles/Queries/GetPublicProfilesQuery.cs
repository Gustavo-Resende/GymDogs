using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.Profiles.Specification;

namespace GymDogs.Application.Profiles.Queries;

public record GetPublicProfilesQuery : IQuery<Result<IEnumerable<GetProfileDto>>>;

internal class GetPublicProfilesQueryHandler : IQueryHandler<GetPublicProfilesQuery, Result<IEnumerable<GetProfileDto>>>
{
    private readonly IReadRepository<Profile> _profileRepository;

    public GetPublicProfilesQueryHandler(IReadRepository<Profile> profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<Result<IEnumerable<GetProfileDto>>> Handle(
        GetPublicProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var profiles = await _profileRepository.ListAsync(
            new GetPublicProfilesSpec(),
            cancellationToken);

        var profileDtos = profiles.Select(p => p.ToGetProfileDto());

        return Result.Success(profileDtos);
    }
}
