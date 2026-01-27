using GymDogs.Application.Profiles.Commands;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Application.Profiles.Queries;
using GymDogs.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GymDogs.Presentation.Controllers;

/// <summary>
/// Controller for managing user profiles.
/// Provides endpoints for retrieving, updating, and searching user profiles.
/// </summary>
[ApiController]
[Route("api/profiles")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the ProfilesController.
    /// </summary>
    /// <param name="mediator">The MediatR mediator instance</param>
    public ProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a profile by ID.
    /// Respects profile visibility settings (public/private).
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The profile data</returns>
    /// <response code="200">Profile found</response>
    /// <response code="404">Profile not found</response>
    [HttpGet("{profileId}")]
    [ProducesResponseType(typeof(GetProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProfileDto>> GetProfileById(
        [FromRoute] Guid profileId,
        CancellationToken cancellationToken)
    {
        var currentUserId = HttpContext.GetUserId();
        var query = new GetProfileByIdQuery(profileId, currentUserId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Gets a profile by user ID.
    /// Respects profile visibility settings (public/private).
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The profile data</returns>
    /// <response code="200">Profile found</response>
    /// <response code="404">Profile not found</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(GetProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProfileDto>> GetProfileByUserId(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var currentUserId = HttpContext.GetUserId();
        var query = new GetProfileByUserIdQuery(userId, currentUserId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lists all public profiles.
    /// Returns a standardized response with information about empty results.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of public profiles with information about empty results</returns>
    /// <response code="200">List of public profiles returned successfully (may be empty)</response>
    [HttpGet("public")]
    [ProducesResponseType(typeof(GetProfilesResponseDto), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "*" })]
    public async Task<ActionResult<GetProfilesResponseDto>> GetPublicProfiles(
        CancellationToken cancellationToken)
    {
        var query = new GetPublicProfilesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Searches for public profiles by username or display name (case-insensitive).
    /// Returns a standardized response with information about empty results.
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching public profiles with information about empty results</returns>
    /// <response code="200">List of public profiles returned successfully (may be empty)</response>
    /// <response code="400">Invalid search term</response>
    [HttpGet("public/search")]
    [ProducesResponseType(typeof(GetProfilesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "searchTerm" })]
    public async Task<ActionResult<GetProfilesResponseDto>> SearchPublicProfiles(
        [FromQuery] string searchTerm,
        CancellationToken cancellationToken)
    {
        var query = new SearchPublicProfilesQuery(searchTerm);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Updates profile data.
    /// Only the profile owner can update their own profile.
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="request">Data to be updated</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated profile</returns>
    /// <response code="200">Profile updated successfully</response>
    /// <response code="400">Invalid data</response>
    /// <response code="404">Profile not found</response>
    [HttpPut("{profileId}")]
    [ProducesResponseType(typeof(GetProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProfileDto>> UpdateProfile(
        [FromRoute] Guid profileId,
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = HttpContext.GetUserId();
        var command = new UpdateProfileCommand(profileId, request.DisplayName, request.Bio, currentUserId);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Updates profile visibility (public/private).
    /// Only the profile owner can update their own profile visibility.
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="request">New visibility setting</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated profile</returns>
    /// <response code="200">Visibility updated successfully</response>
    /// <response code="400">Invalid data</response>
    /// <response code="404">Profile not found</response>
    [HttpPut("{profileId}/visibility")]
    [ProducesResponseType(typeof(GetProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProfileDto>> UpdateProfileVisibility(
        [FromRoute] Guid profileId,
        [FromBody] UpdateProfileVisibilityRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = HttpContext.GetUserId();
        var visibilityDto = request.Visibility;
        var command = new UpdateProfileVisibilityCommand(profileId, visibilityDto, currentUserId);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request DTO for updating a profile.
/// </summary>
public record UpdateProfileRequest
{
    /// <summary>
    /// Profile display name.
    /// </summary>
    /// <example>John Doe</example>
    [StringLength(200, ErrorMessage = "DisplayName must have a maximum of 200 characters")]
    public string? DisplayName { get; init; }

    /// <summary>
    /// Profile biography (maximum 1000 characters).
    /// </summary>
    /// <example>Fitness enthusiast and healthy lifestyle advocate</example>
    [StringLength(1000, ErrorMessage = "Bio must have a maximum of 1000 characters")]
    public string? Bio { get; init; }
}

/// <summary>
/// Request DTO for updating profile visibility.
/// </summary>
public record UpdateProfileVisibilityRequest
{
    /// <summary>
    /// New profile visibility (1 = Public, 2 = Private).
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "Visibility is required")]
    public ProfileVisibilityDto Visibility { get; init; }
}
