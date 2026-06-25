namespace ImprentaSR.WebAPI.Models;

/// <summary>
/// DTO de respuesta para operaciones de autenticación.
/// </summary>
public record AuthResponse
{
    /// <summary>Token JWT de acceso.</summary>
    public string Token { get; init; } = string.Empty;

    /// <summary>Nombre del usuario autenticado.</summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>Correo electrónico del usuario.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Rol del usuario.</summary>
    public string Rol { get; init; } = string.Empty;
}
