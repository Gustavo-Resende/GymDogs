using System;
using System.Collections.Generic;
using System.Text;

namespace GymDogs.Application.Users.Dtos
{
    /// <summary>
    /// DTO básico para listagem e leitura de usuário
    /// </summary>
    public record GetUserDto(
        Guid Id,
        string Username,
        string Email,
        DateTime CreatedAt,
        DateTime LastUpdatedAt
    );
}
