using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.ExerciseSets.Dtos;
using GymDogs.Application.ExerciseSets.Extensions;
using GymDogs.Domain.ExerciseSets;
using GymDogs.Domain.FolderExercises;

namespace GymDogs.Application.ExerciseSets.Commands;

public record AddExerciseSetCommand(Guid FolderExerciseId, int? SetNumber, int Reps, decimal Weight, Guid? CurrentUserId = null)
    : ICommand<Result<CreateExerciseSetDto>>;

internal class AddExerciseSetCommandHandler : ICommandHandler<AddExerciseSetCommand, Result<CreateExerciseSetDto>>
{
    private readonly IRepository<ExerciseSet> _exerciseSetRepository;
    private readonly IReadRepository<FolderExercise> _folderExerciseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecificationFactory _specificationFactory;

    public AddExerciseSetCommandHandler(
        IRepository<ExerciseSet> exerciseSetRepository,
        IReadRepository<FolderExercise> folderExerciseRepository,
        IUnitOfWork unitOfWork,
        ISpecificationFactory specificationFactory)
    {
        _exerciseSetRepository = exerciseSetRepository;
        _folderExerciseRepository = folderExerciseRepository;
        _unitOfWork = unitOfWork;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<CreateExerciseSetDto>> Handle(
        AddExerciseSetCommand request,
        CancellationToken cancellationToken)
    {
        if (request.FolderExerciseId == Guid.Empty)
        {
            return Result<CreateExerciseSetDto>.NotFound("FolderExercise ID is required.");
        }

        var folderExercise = await _folderExerciseRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetFolderExerciseByIdSpec(request.FolderExerciseId),
            cancellationToken);

        if (folderExercise == null)
        {
            return Result<CreateExerciseSetDto>.NotFound($"FolderExercise with ID {request.FolderExerciseId} not found.");
        }

        // Property-based authorization: Users can only add sets to exercises in their own workout folders
        if (!request.CurrentUserId.HasValue || folderExercise.WorkoutFolder.Profile.UserId != request.CurrentUserId.Value)
        {
            return Result<CreateExerciseSetDto>.Forbidden("You can only add sets to exercises in your own workout folders.");
        }

        var setNumber = request.SetNumber ?? await GetNextSetNumber(request.FolderExerciseId, cancellationToken);

        var exerciseSet = new ExerciseSet(
            request.FolderExerciseId,
            setNumber,
            request.Reps,
            request.Weight);

        await _exerciseSetRepository.AddAsync(exerciseSet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(exerciseSet.ToCreateExerciseSetDto());
    }

    private async Task<int> GetNextSetNumber(Guid folderExerciseId, CancellationToken cancellationToken)
    {
        var existingSets = await _exerciseSetRepository.ListAsync(
            _specificationFactory.CreateGetExerciseSetsByFolderExerciseIdSpec(folderExerciseId),
            cancellationToken);

        if (!existingSets.Any())
        {
            return 1;
        }

        return existingSets.Max(es => es.SetNumber) + 1;
    }
}
