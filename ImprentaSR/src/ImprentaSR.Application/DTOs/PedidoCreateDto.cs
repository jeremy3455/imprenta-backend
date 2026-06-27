namespace ImprentaSR.Application.DTOs;

public record PedidoCreateDto
{
    public int ClienteId { get; init; }
    public string FormaPago { get; init; } = string.Empty;
    public decimal MontoAnticipo { get; init; }
    public DateTime? FechaVencimientoCredito { get; init; }
    public string? Observaciones { get; init; }
    public List<DetallePedidoCreateDto> Items { get; init; } = new();
}

public record DetallePedidoCreateDto
{
    public int ProductoId { get; init; }
    public int Cantidad { get; init; }
    public decimal PrecioUnitario { get; init; }
    public string? NumeroAutorizacionSri { get; init; }
    public string? SeriePrincipal { get; init; }
    public string? SerieSecundaria { get; init; }
    public int? SecuencialDesde { get; init; }
    public int? SecuencialHasta { get; init; }
}
