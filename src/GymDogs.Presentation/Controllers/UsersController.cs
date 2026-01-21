using Ardalis.Result;
using GymDogs.Application.Users.Commands;
using GymDogs.Application.Users.Dtos;
using GymDogs.Application.Users.Queries;
using GymDogs.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
    /// Cria um novo usuário no sistema
    /// </summary>
    /// <param name="request">Dados do usuário a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Usuário criado com sucesso</returns>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">Dados inválidos fornecidos</response>
    /// <response code="409">Email ou username já existe no sistema</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CreateUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateUserDto>> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            request.Username,
            request.Email,
            request.Password
        );

        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
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
    /// Lista todos os usuários do sistema
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de usuários</returns>
    /// <response code="200">Lista de usuários retornada com sucesso</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetUserDto>), StatusCodes.Status200OK)]
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
        [FromBody] UpdateUserEmailRequest request,
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
        [FromBody] UpdateUserUsernameRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserUsernameCommand(id, request.Username);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deleta um usuário do sistema
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de exclusão</returns>
    /// <response code="204">Usuário deletado com sucesso</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request DTO para criação de usuário
/// </summary>
public record CreateUserRequest
{
    /// <summary>
    /// Nome de usuário único (máximo 100 caracteres)
    /// </summary>
    /// <example>johndoe</example>
    [Required(ErrorMessage = "Username é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username deve ter entre 3 e 100 caracteres")]
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// Email único do usuário
    /// </summary>
    /// <example>john@example.com</example>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Senha do usuário (será hasheada antes de salvar)
    /// </summary>
    /// <example>SecurePassword123!</example>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    [MaxLength(100, ErrorMessage = "Senha deve ter no máximo 100 caracteres")]
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Request DTO para atualização de email
/// </summary>
public record UpdateUserEmailRequest
{
    /// <summary>
    /// Novo email do usuário
    /// </summary>
    /// <example>newemail@example.com</example>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
    public string Email { get; init; } = string.Empty;
}

/// <summary>
/// Request DTO para atualização de username
/// </summary>
public record UpdateUserUsernameRequest
{
    /// <summary>
    /// Novo username do usuário
    /// </summary>
    /// <example>newusername</example>
    [Required(ErrorMessage = "Username é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username deve ter entre 3 e 100 caracteres")]
    public string Username { get; init; } = string.Empty;
}
