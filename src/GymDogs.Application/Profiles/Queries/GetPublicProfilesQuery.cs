using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Domain.Profiles;

namespace GymDogs.Application.Profiles.Queries;

public record GetPublicProfilesQuery : IQuery<Result<IEnumerable<GetProfileDto>>>;

internal class GetPublicProfilesQueryHandler : IQueryHandler<GetPublicProfilesQuery, Result<IEnumerable<GetProfileDto>>>
{
    private readonly IReadRepository<Profile> _profileRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public GetPublicProfilesQueryHandler(
        IReadRepository<Profile> profileRepository,
        ISpecificationFactory specificationFactory)
    {
        _profileRepository = profileRepository;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<IEnumerable<GetProfileDto>>> Handle(
        GetPublicProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var profiles = await _profileRepository.ListAsync(
            _specificationFactory.CreateGetPublicProfilesSpec(),
            cancellationToken);

        var profileDtos = profiles.Select(p => p.ToGetProfileDto());

        return Result.Success(profileDtos);
    }
}
