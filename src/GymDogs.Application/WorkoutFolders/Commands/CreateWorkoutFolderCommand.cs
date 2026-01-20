using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Dtos;
using GymDogs.Application.WorkoutFolders.Extensions;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.Profiles.Specification;
using GymDogs.Domain.WorkoutFolders;

namespace GymDogs.Application.WorkoutFolders.Commands;

public record CreateWorkoutFolderCommand(Guid ProfileId, string Name, string? Description, int Order = 0)
    : ICommand<Result<CreateWorkoutFolderDto>>;

internal class CreateWorkoutFolderCommandHandler : ICommandHandler<CreateWorkoutFolderCommand, Result<CreateWorkoutFolderDto>>
{
    private readonly IRepository<WorkoutFolder> _workoutFolderRepository;
    private readonly IReadRepository<Profile> _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWorkoutFolderCommandHandler(
        IRepository<WorkoutFolder> workoutFolderRepository,
        IReadRepository<Profile> profileRepository,
        IUnitOfWork unitOfWork)
    {
        _workoutFolderRepository = workoutFolderRepository;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateWorkoutFolderDto>> Handle(
        CreateWorkoutFolderCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ProfileId == Guid.Empty)
        {
            return Result<CreateWorkoutFolderDto>.NotFound("Profile ID is required.");
        }

        var profile = await _profileRepository.FirstOrDefaultAsync(
            new GetProfileByIdSpec(request.ProfileId),
            cancellationToken);

        if (profile == null)
        {
            return Result<CreateWorkoutFolderDto>.NotFound($"Profile with ID {request.ProfileId} not found.");
        }

        var folder = new WorkoutFolder(
            request.ProfileId,
            request.Name?.Trim() ?? string.Empty,
            request.Description?.Trim(),
            request.Order);
        await _workoutFolderRepository.AddAsync(folder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(folder.ToCreateWorkoutFolderDto());
    }
}
