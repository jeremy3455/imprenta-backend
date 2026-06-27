namespace ImprentaSR.WebAPI.Models;

public record AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Rol { get; init; } = string.Empty;
    public int? ClienteId { get; init; }
}
