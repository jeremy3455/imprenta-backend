namespace ImprentaSR.Application.DTOs;

public record SolicitudDetalleDto
{
    public int Id { get; init; }
    public string RazonSocialCliente { get; init; } = string.Empty;
    public string NumeroCedulaRuc { get; init; } = string.Empty;
    public int ClienteId { get; init; }
    public string Estado { get; init; } = string.Empty;
    public string FormaPago { get; init; } = string.Empty;
    public int? PedidoId { get; init; }
    public decimal? MontoTotal { get; init; }
    public string? Observacion { get; init; }
    public DateTime FechaSolicitud { get; init; }
    public IReadOnlyList<SolicitudDetalleItemDto> Items { get; init; } = Array.Empty<SolicitudDetalleItemDto>();
}

public record SolicitudDetalleItemDto
{
    public int Id { get; init; }
    public int ProductoId { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public int Cantidad { get; init; }
    public string? Observacion { get; init; }
}
