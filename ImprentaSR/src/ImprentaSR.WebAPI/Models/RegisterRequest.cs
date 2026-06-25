namespace ImprentaSR.WebAPI.Models;

/// <summary>
/// DTO de entrada para el registro de un nuevo usuario.
/// </summary>
public record RegisterRequest
{
    /// <summary>Nombre completo del usuario.</summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>Correo electrónico.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Contraseña.</summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>Rol del usuario (Admin, Operador, Cliente).</summary>
    public string Rol { get; init; } = "Cliente";
}
