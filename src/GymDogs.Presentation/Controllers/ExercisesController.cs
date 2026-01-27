using GymDogs.Application.Common;
using GymDogs.Application.Exercises.Commands;
using GymDogs.Application.Exercises.Dtos;
using GymDogs.Application.Exercises.Queries;
using GymDogs.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GymDogs.Presentation.Controllers;

/// <summary>
/// Controller for managing the exercise catalog.
/// Provides endpoints for CRUD operations on exercises.
/// </summary>
[ApiController]
[Route("api/exercises")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[Authorize]
public class ExercisesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the ExercisesController.
    /// </summary>
    /// <param name="mediator">The MediatR mediator instance</param>
    public ExercisesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new exercise in the catalog (Admin only).
    /// </summary>
    /// <param name="request">Exercise data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created exercise</returns>
    /// <response code="201">Exercise created successfully</response>
    /// <response code="400">Invalid data</response>
    /// <response code="403">Access denied - administrators only</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(CreateExerciseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CreateExerciseDto>> CreateExercise(
        [FromBody] CreateExerciseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateExerciseCommand(request.Name, request.Description);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lists all exercises in the catalog.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of exercises</returns>
    /// <response code="200">List of exercises returned successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetExerciseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetExerciseDto>>> GetAllExercises(
        CancellationToken cancellationToken)
    {
        var query = new GetAllExercisesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Searches for exercises by name (case-insensitive).
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching exercises</returns>
    /// <response code="200">List of exercises returned successfully</response>
    /// <response code="400">Invalid search term</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<GetExerciseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<GetExerciseDto>>> SearchExercisesByName(
        [FromQuery] string searchTerm,
        CancellationToken cancellationToken)
    {
        var query = new SearchExercisesByNameQuery(searchTerm);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lists exercises available (not added) for a workout folder.
    /// Returns all exercises that are not yet associated with the specified workout folder.
    /// </summary>
    /// <param name="workoutFolderId">The workout folder ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available exercises</returns>
    /// <response code="200">List of available exercises returned successfully</response>
    /// <response code="400">Invalid folder ID</response>
    [HttpGet("available/{workoutFolderId}")]
    [ProducesResponseType(typeof(IEnumerable<GetExerciseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<GetExerciseDto>>> GetAvailableExercisesForFolder(
        [FromRoute] Guid workoutFolderId,
        CancellationToken cancellationToken)
    {
        var query = new GetAvailableExercisesForFolderQuery(workoutFolderId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Searches for exercises available (not added) for a workout folder by name (case-insensitive).
    /// </summary>
    /// <param name="workoutFolderId">The workout folder ID</param>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching available exercises</returns>
    /// <response code="200">List of available exercises returned successfully</response>
    /// <response code="400">Invalid folder ID or search term</response>
    [HttpGet("available/{workoutFolderId}/search")]
    [ProducesResponseType(typeof(IEnumerable<GetExerciseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<GetExerciseDto>>> SearchAvailableExercisesForFolder(
        [FromRoute] Guid workoutFolderId,
        [FromQuery] string searchTerm,
        CancellationToken cancellationToken)
    {
        var query = new SearchAvailableExercisesForFolderQuery(workoutFolderId, searchTerm);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Gets an exercise by ID.
    /// </summary>
    /// <param name="id">The exercise ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The exercise data</returns>
    /// <response code="200">Exercise found</response>
    /// <response code="404">Exercise not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetExerciseDto>> GetExerciseById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetExerciseByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Updates an exercise (Admin only).
    /// </summary>
    /// <param name="id">The exercise ID</param>
    /// <param name="request">Data to be updated</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated exercise</returns>
    /// <response code="200">Exercise updated successfully</response>
    /// <response code="400">Invalid data</response>
    /// <response code="403">Access denied - administrators only</response>
    /// <response code="404">Exercise not found</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(GetExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetExerciseDto>> UpdateExercise(
        [FromRoute] Guid id,
        [FromBody] UpdateExerciseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateExerciseCommand(id, request.Name, request.Description);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deletes an exercise from the catalog (Admin only).
    /// </summary>
    /// <param name="id">The exercise ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    /// <response code="204">Exercise deleted successfully</response>
    /// <response code="403">Access denied - administrators only</response>
    /// <response code="404">Exercise not found</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteExercise(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteExerciseCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request DTO for creating an exercise.
/// </summary>
public record CreateExerciseRequest
{
    /// <summary>
    /// Exercise name (maximum 200 characters).
    /// </summary>
    /// <example>Bench Press</example>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Exercise description (optional, maximum 1000 characters).
    /// </summary>
    /// <example>Exercise for chest development</example>
    [StringLength(1000, ErrorMessage = "Description must have a maximum of 1000 characters")]
    public string? Description { get; init; }
}

/// <summary>
/// Request DTO for updating an exercise.
/// </summary>
public record UpdateExerciseRequest
{
    /// <summary>
    /// New exercise name.
    /// </summary>
    /// <example>Bench Press with Bar</example>
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters")]
    public string? Name { get; init; }

    /// <summary>
    /// New exercise description.
    /// </summary>
    /// <example>Exercise for chest development using a bar</example>
    [StringLength(1000, ErrorMessage = "Description must have a maximum of 1000 characters")]
    public string? Description { get; init; }
}
