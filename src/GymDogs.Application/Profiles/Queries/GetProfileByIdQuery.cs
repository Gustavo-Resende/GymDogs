using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Domain.Profiles;

namespace GymDogs.Application.Profiles.Queries;

public record GetProfileByIdQuery(Guid ProfileId, Guid? CurrentUserId = null) : IQuery<Result<GetProfileDto>>;

internal class GetProfileByIdQueryHandler : IQueryHandler<GetProfileByIdQuery, Result<GetProfileDto>>
{
    private readonly IReadRepository<Profile> _profileRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public GetProfileByIdQueryHandler(
        IReadRepository<Profile> profileRepository,
        ISpecificationFactory specificationFactory)
    {
        _profileRepository = profileRepository;
        _specificationFactory = specificationFactory;
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
            _specificationFactory.CreateGetProfileByIdSpec(request.ProfileId),
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
