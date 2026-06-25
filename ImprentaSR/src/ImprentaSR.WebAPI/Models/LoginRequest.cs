namespace ImprentaSR.WebAPI.Models;

/// <summary>
/// DTO de entrada para la solicitud de inicio de sesión.
/// </summary>
public record LoginRequest
{
    /// <summary>Correo electrónico del usuario.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Contraseña del usuario.</summary>
    public string Password { get; init; } = string.Empty;
}
