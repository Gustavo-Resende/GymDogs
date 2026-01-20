using System;
using System.Collections.Generic;
using System.Text;

namespace GymDogs.Application.Users.Dtos
{
    /// <summary>
    /// DTO retornado após criar um usuário
    /// </summary>
    public record CreateUserDto(
        Guid Id,
        string Username,
        string Email,
        DateTime CreatedAt
    );
}
