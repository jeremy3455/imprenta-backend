namespace ImprentaSR.Application.DTOs;

public record SolicitudResumenDto
{
    public int Id { get; init; }
    public string RazonSocialCliente { get; init; } = string.Empty;
    public int ClienteId { get; init; }
    public string Estado { get; init; } = string.Empty;
    public string? Observacion { get; init; }
    public DateTime FechaSolicitud { get; init; }
    public int CantidadItems { get; init; }
}
