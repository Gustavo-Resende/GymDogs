using GymDogs.Application.Profiles.Commands;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Extensions;
using GymDogs.Application.Profiles.Queries;
using GymDogs.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GymDogs.Presentation.Controllers;

/// <summary>
/// Controller para gerenciamento de perfis de usuários
/// </summary>
[ApiController]
[Route("api/profiles")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém um perfil pelo ID
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do perfil</returns>
    /// <response code="200">Perfil encontrado</response>
    /// <response code="404">Perfil não encontrado</response>
    [HttpGet("{profileId}")]
    [ProducesResponseType(typeof(GetProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProfileDto>> GetProfileById(
        [FromRoute] Guid profileId,
        CancellationToken cancellationToken)
    {
        var query = new GetProfileByIdQuery(profileId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Obtém um perfil pelo ID do usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do perfil</returns>
    /// <response code="200">Perfil encontrado</response>
    /// <response code="404">Perfil não encontrado</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(GetProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProfileDto>> GetProfileByUserId(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new GetProfileByUserIdQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista todos os perfis públicos
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de perfis públicos</returns>
    /// <response code="200">Lista de perfis públicos retornada com sucesso</response>
    [HttpGet("public")]
    [ProducesResponseType(typeof(IEnumerable<GetProfileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetProfileDto>>> GetPublicProfiles(
        CancellationToken cancellationToken)
    {
        var query = new GetPublicProfilesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Atualiza os dados de um perfil
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="request">Dados a serem atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Perfil atualizado</returns>
    /// <response code="200">Perfil atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Perfil não encontrado</response>
    [HttpPut("{profileId}")]
    [ProducesResponseType(typeof(GetProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProfileDto>> UpdateProfile(
        [FromRoute] Guid profileId,
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProfileCommand(profileId, request.DisplayName, request.Bio);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Atualiza a visibilidade de um perfil
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="request">Nova visibilidade</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Perfil atualizado</returns>
    /// <response code="200">Visibilidade atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Perfil não encontrado</response>
    [HttpPut("{profileId}/visibility")]
    [ProducesResponseType(typeof(GetProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProfileDto>> UpdateProfileVisibility(
        [FromRoute] Guid profileId,
        [FromBody] UpdateProfileVisibilityRequest request,
        CancellationToken cancellationToken)
    {
        var visibilityDto = request.Visibility;
        var command = new UpdateProfileVisibilityCommand(profileId, visibilityDto);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request DTO para atualização de perfil
/// </summary>
public record UpdateProfileRequest
{
    /// <summary>
    /// Nome de exibição do perfil
    /// </summary>
    /// <example>João Silva</example>
    [StringLength(200, ErrorMessage = "DisplayName deve ter no máximo 200 caracteres")]
    public string? DisplayName { get; init; }

    /// <summary>
    /// Biografia do perfil (máximo 1000 caracteres)
    /// </summary>
    /// <example>Entusiasta de musculação e vida saudável</example>
    [StringLength(1000, ErrorMessage = "Bio deve ter no máximo 1000 caracteres")]
    public string? Bio { get; init; }
}

/// <summary>
/// Request DTO para atualização de visibilidade do perfil
/// </summary>
public record UpdateProfileVisibilityRequest
{
    /// <summary>
    /// Nova visibilidade do perfil (1 = Public, 2 = Private)
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "Visibility é obrigatório")]
    public ProfileVisibilityDto Visibility { get; init; }
}
