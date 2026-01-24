using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.Profiles.Specification;

namespace GymDogs.Application.Profiles.Queries;

public record SearchPublicProfilesQuery(string SearchTerm) : IQuery<Result<IEnumerable<GetProfileDto>>>;

internal class SearchPublicProfilesQueryHandler : IQueryHandler<SearchPublicProfilesQuery, Result<IEnumerable<GetProfileDto>>>
{
    private readonly IReadRepository<Profile> _profileRepository;

    public SearchPublicProfilesQueryHandler(IReadRepository<Profile> profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<Result<IEnumerable<GetProfileDto>>> Handle(
        SearchPublicProfilesQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return Result<IEnumerable<GetProfileDto>>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = "SearchTerm", ErrorMessage = "Search term is required." }
                });
        }

        var profiles = await _profileRepository.ListAsync(
            new SearchPublicProfilesSpec(request.SearchTerm.Trim()),
            cancellationToken);

        var profileDtos = profiles.Select(p => p.ToGetProfileDto());

        return Result.Success(profileDtos);
    }
}
