using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GymDogs.Presentation.Extensions;

/// <summary>
/// Extensions for the HttpContext class to extract user information from JWT claims
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Gets the user ID from the claims of the HTTP context
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>The user ID as Guid or null if not found</returns>
    public static Guid? GetUserId(this HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) 
            ? userId 
            : null;
    }

    /// <summary>
    /// Gets the user role from the claims of the HTTP context
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>The user role as string or null if not found</returns>
    public static string? GetUserRole(this HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.Role)?.Value;
    }
}