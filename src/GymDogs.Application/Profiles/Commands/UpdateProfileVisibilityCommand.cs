using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Domain.Profiles;

namespace GymDogs.Application.Profiles.Commands;

public record UpdateProfileVisibilityCommand(Guid ProfileId, ProfileVisibilityDto Visibility, Guid? CurrentUserId)
    : ICommand<Result<GetProfileDto>>;

internal class UpdateProfileVisibilityCommandHandler : ICommandHandler<UpdateProfileVisibilityCommand, Result<GetProfileDto>>
{
    private readonly IRepository<Profile> _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecificationFactory _specificationFactory;

    public UpdateProfileVisibilityCommandHandler(
        IRepository<Profile> profileRepository,
        IUnitOfWork unitOfWork,
        ISpecificationFactory specificationFactory)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<GetProfileDto>> Handle(
        UpdateProfileVisibilityCommand request,
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

        // Property-based authorization: Users can only update their own profile visibility
        if (!request.CurrentUserId.HasValue || profile.UserId != request.CurrentUserId.Value)
        {
            return Result<GetProfileDto>.Forbidden("You can only update your own profile visibility.");
        }

        var visibility = request.Visibility.ToProfileVisibility();
        profile.UpdateVisibility(visibility);
        await _profileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(profile.ToGetProfileDto());
    }
}
