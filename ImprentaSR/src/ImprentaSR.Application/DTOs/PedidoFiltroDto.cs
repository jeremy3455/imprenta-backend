namespace ImprentaSR.Application.DTOs;

public record PedidoFiltroDto
{
    public string? Estado { get; init; }
    public int? ClienteId { get; init; }
    public string? FormaPago { get; init; }
    public DateTime? FechaDesde { get; init; }
    public DateTime? FechaHasta { get; init; }
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
