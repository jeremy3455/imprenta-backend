namespace ImprentaSR.Application.DTOs;

public record PedidoDetalleDto
{
    public int Id { get; init; }
    public int ClienteId { get; init; }
    public string RazonSocialCliente { get; init; } = string.Empty;
    public string NumeroCedulaRuc { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string FormaPago { get; init; } = string.Empty;
    public decimal MontoTotal { get; init; }
    public decimal MontoAnticipo { get; init; }
    public decimal MontoPendiente { get; init; }
    public DateTime? FechaVencimientoCredito { get; init; }
    public string? Observaciones { get; init; }
    public DateTime FechaRegistro { get; init; }
    public DateTime? FechaAprobacion { get; init; }
    public DateTime? FechaInicioProduccion { get; init; }
    public DateTime? FechaListoEntrega { get; init; }
    public DateTime? FechaEntrega { get; init; }
    public DateTime? FechaAnulacion { get; init; }
    public string? MotivoAnulacion { get; init; }
    public IReadOnlyList<DetallePedidoDto> Detalles { get; init; } = Array.Empty<DetallePedidoDto>();
}
