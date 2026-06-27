namespace ImprentaSR.Application.DTOs;

public record SolicitudCreateDto
{
    public string FormaPago { get; init; } = "EFECTIVO";
    public string? Observacion { get; init; }
    public List<SolicitudItemDto> Items { get; init; } = new();
}

public record SolicitudItemDto
{
    public int ProductoId { get; init; }
    public int Cantidad { get; init; }
    public string? Observacion { get; init; }
}
