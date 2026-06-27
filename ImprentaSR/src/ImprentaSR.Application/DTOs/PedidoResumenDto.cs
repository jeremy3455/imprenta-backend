namespace ImprentaSR.Application.DTOs;

public record PedidoResumenDto
{
    public int Id { get; init; }
    public int ClienteId { get; init; }
    public string RazonSocialCliente { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string FormaPago { get; init; } = string.Empty;
    public decimal MontoTotal { get; init; }
    public decimal MontoPendiente { get; init; }
    public DateTime FechaRegistro { get; init; }
    public int CantidadItems { get; init; }
}
