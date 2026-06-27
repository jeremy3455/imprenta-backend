namespace ImprentaSR.WebAPI.Models;

/// <summary>
/// DTO de entrada para el registro de un nuevo usuario.
/// </summary>
public record RegisterRequest
{
    public string Nombre { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Rol { get; init; } = "Cliente";
    public int? ClienteId { get; init; }
}
