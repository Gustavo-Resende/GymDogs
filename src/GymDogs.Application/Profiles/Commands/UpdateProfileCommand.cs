using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Domain.Profiles;

namespace GymDogs.Application.Profiles.Commands;

public record UpdateProfileCommand(Guid ProfileId, string? DisplayName, string? Bio, Guid? CurrentUserId)
    : ICommand<Result<GetProfileDto>>;

internal class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, Result<GetProfileDto>>
{
    private readonly IRepository<Profile> _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecificationFactory _specificationFactory;

    public UpdateProfileCommandHandler(
        IRepository<Profile> profileRepository,
        IUnitOfWork unitOfWork,
        ISpecificationFactory specificationFactory)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<GetProfileDto>> Handle(
        UpdateProfileCommand request,
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

        // Property-based authorization: Users can only update their own profile
        if (!request.CurrentUserId.HasValue || profile.UserId != request.CurrentUserId.Value)
        {
            return Result<GetProfileDto>.Forbidden("You can only update your own profile.");
        }

        profile.UpdateProfile(request.DisplayName, request.Bio);
        await _profileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(profile.ToGetProfileDto());
    }
}
