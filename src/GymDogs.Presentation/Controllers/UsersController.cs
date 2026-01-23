using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Users.Commands;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Queries;
using GymDogs.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymDogs.Presentation.Controllers;

/// <summary>
/// Controller para gerenciamento de usuários
/// </summary>
[ApiController]
[Route("api/users")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém um usuário pelo ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do usuário</returns>
    /// <response code="200">Usuário encontrado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserDto>> GetUserById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Obtém um usuário pelo email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do usuário</returns>
    /// <response code="200">Usuário encontrado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserDto>> GetUserByEmail(
        [FromRoute] string email,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByEmailQuery(email);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Obtém um usuário pelo username
    /// </summary>
    /// <param name="username">Username do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do usuário</returns>
    /// <response code="200">Usuário encontrado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpGet("username/{username}")]
    [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserDto>> GetUserByUsername(
        [FromRoute] string username,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByUsernameQuery(username);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista todos os usuários do sistema (Apenas Admin)
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de usuários</returns>
    /// <response code="200">Lista de usuários retornada com sucesso</response>
    /// <response code="403">Acesso negado - apenas administradores</response>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(IEnumerable<GetUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<GetUserDto>>> GetAllUsers(
        CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Atualiza o email de um usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="request">Novo email</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Usuário atualizado</returns>
    /// <response code="200">Email atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="409">Email já está em uso por outro usuário</response>
    [HttpPut("{id}/email")]
    [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetUserDto>> UpdateUserEmail(
        [FromRoute] Guid id,
        [FromBody] UpdateUserEmailDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserEmailCommand(id, request.Email);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Atualiza o username de um usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="request">Novo username</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Usuário atualizado</returns>
    /// <response code="200">Username atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="409">Username já está em uso por outro usuário</response>
    [HttpPut("{id}/username")]
    [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetUserDto>> UpdateUserUsername(
        [FromRoute] Guid id,
        [FromBody] UpdateUserUsernameDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserUsernameCommand(id, request.Username);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deleta um usuário do sistema (Admin pode deletar qualquer um, User pode deletar apenas a si mesmo)
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de exclusão</returns>
    /// <response code="204">Usuário deletado com sucesso</response>
    /// <response code="403">Acesso negado - você só pode deletar sua própria conta</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var currentUserId = HttpContext.GetUserId();
        var currentUserRole = HttpContext.GetUserRole();
        
        var command = new DeleteUserCommand(id, currentUserId, currentUserRole);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}