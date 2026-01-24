using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Domain.Profiles;

namespace GymDogs.Application.Profiles.Queries;

public record GetProfileByUserIdQuery(Guid UserId, Guid? CurrentUserId = null) : IQuery<Result<GetProfileDto>>;

internal class GetProfileByUserIdQueryHandler : IQueryHandler<GetProfileByUserIdQuery, Result<GetProfileDto>>
{
    private readonly IReadRepository<Profile> _profileRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public GetProfileByUserIdQueryHandler(
        IReadRepository<Profile> profileRepository,
        ISpecificationFactory specificationFactory)
    {
        _profileRepository = profileRepository;
        _specificationFactory = specificationFactory;
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
            _specificationFactory.CreateGetProfileByUserIdSpec(request.UserId),
            cancellationToken);

        if (profile == null)
        {
            return Result<GetProfileDto>.NotFound($"Profile for User ID {request.UserId} not found.");
        }

        if (!profile.IsVisibleTo(request.CurrentUserId))
        {
            return Result<GetProfileDto>.Forbidden("You do not have permission to view this profile.");
        }

        return Result.Success(profile.ToGetProfileDto());
    }
}
