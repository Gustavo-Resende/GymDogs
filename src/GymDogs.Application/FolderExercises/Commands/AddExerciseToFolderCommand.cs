using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.FolderExercises.Dtos;
using GymDogs.Application.FolderExercises.Extensions;
using GymDogs.Domain.Exercises;
using GymDogs.Domain.Exercises.Specification;
using GymDogs.Domain.FolderExercises;
using GymDogs.Domain.FolderExercises.Specification;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Domain.WorkoutFolders.Specification;

namespace GymDogs.Application.FolderExercises.Commands;

public record AddExerciseToFolderCommand(Guid WorkoutFolderId, Guid ExerciseId, int Order = 0)
    : ICommand<Result<AddExerciseToFolderDto>>;

internal class AddExerciseToFolderCommandHandler : ICommandHandler<AddExerciseToFolderCommand, Result<AddExerciseToFolderDto>>
{
    private readonly IRepository<FolderExercise> _folderExerciseRepository;
    private readonly IReadRepository<WorkoutFolder> _workoutFolderRepository;
    private readonly IReadRepository<Exercise> _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddExerciseToFolderCommandHandler(
        IRepository<FolderExercise> folderExerciseRepository,
        IReadRepository<WorkoutFolder> workoutFolderRepository,
        IReadRepository<Exercise> exerciseRepository,
        IUnitOfWork unitOfWork)
    {
        _folderExerciseRepository = folderExerciseRepository;
        _workoutFolderRepository = workoutFolderRepository;
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddExerciseToFolderDto>> Handle(
        AddExerciseToFolderCommand request,
        CancellationToken cancellationToken)
    {
        if (request.WorkoutFolderId == Guid.Empty)
        {
            return Result<AddExerciseToFolderDto>.NotFound("WorkoutFolder ID is required.");
        }

        if (request.ExerciseId == Guid.Empty)
        {
            return Result<AddExerciseToFolderDto>.NotFound("Exercise ID is required.");
        }

        var workoutFolder = await _workoutFolderRepository.FirstOrDefaultAsync(
            new GetWorkoutFolderByIdSpec(request.WorkoutFolderId),
            cancellationToken);

        if (workoutFolder == null)
        {
            return Result<AddExerciseToFolderDto>.NotFound($"WorkoutFolder with ID {request.WorkoutFolderId} not found.");
        }

        var exercise = await _exerciseRepository.FirstOrDefaultAsync(
            new GetExerciseByIdSpec(request.ExerciseId),
            cancellationToken);

        if (exercise == null)
        {
            return Result<AddExerciseToFolderDto>.NotFound($"Exercise with ID {request.ExerciseId} not found.");
        }

        var existingFolderExercise = await _folderExerciseRepository.FirstOrDefaultAsync(
            new GetFolderExerciseByFolderAndExerciseSpec(request.WorkoutFolderId, request.ExerciseId),
            cancellationToken);

        if (existingFolderExercise != null)
        {
            return Result<AddExerciseToFolderDto>.Conflict("This exercise is already added to the folder.");
        }

        var folderExercise = new FolderExercise(request.WorkoutFolderId, request.ExerciseId, request.Order);
        await _folderExerciseRepository.AddAsync(folderExercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(folderExercise.ToAddExerciseToFolderDto());
    }
}
