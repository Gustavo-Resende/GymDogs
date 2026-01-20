namespace GymDogs.Application.Users.Dtos;

/// <summary>
/// DTO para atualizar dados do usuário
/// </summary>
public record UpdateUserDto(
    string? Username,
    string? Email
);

/// <summary>
/// DTO para atualizar apenas o email do usuário
/// </summary>
public record UpdateUserEmailDto(
    string Email
);

/// <summary>
/// DTO para atualizar apenas o username do usuário
/// </summary>
public record UpdateUserUsernameDto(
    string Username
);