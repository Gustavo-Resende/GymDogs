using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.Profiles.Specification;

namespace GymDogs.Application.Profiles.Commands;

public record UpdateProfileVisibilityCommand(Guid ProfileId, ProfileVisibilityDto Visibility)
    : ICommand<Result<GetProfileDto>>;

internal class UpdateProfileVisibilityCommandHandler : ICommandHandler<UpdateProfileVisibilityCommand, Result<GetProfileDto>>
{
    private readonly IRepository<Profile> _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileVisibilityCommandHandler(
        IRepository<Profile> profileRepository,
        IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
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
            new GetProfileByIdSpec(request.ProfileId),
            cancellationToken);

        if (profile == null)
        {
            return Result<GetProfileDto>.NotFound($"Profile with ID {request.ProfileId} not found.");
        }

        var visibility = request.Visibility.ToProfileVisibility();
        profile.UpdateVisibility(visibility);
        await _profileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(profile.ToGetProfileDto());
    }
}
