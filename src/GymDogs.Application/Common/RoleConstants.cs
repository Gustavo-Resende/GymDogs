using GymDogs.Domain.Users;

namespace GymDogs.Application.Common;

/// <summary>
/// Constantes de roles sincronizadas com o enum Role do Domain
/// </summary>
public static class RoleConstants
{
    /// <summary>
    /// Role de administrador
    /// </summary>
    public const string Admin = nameof(Role.Admin);
    
    /// <summary>
    /// Role de usuário comum
    /// </summary>
    public const string User = nameof(Role.User);
}