namespace ImprentaSR.Application.DTOs;

public record DetallePedidoDto
{
    public int Id { get; init; }
    public int ProductoId { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public bool EsDocumentoTributario { get; init; }
    public int Cantidad { get; init; }
    public decimal PrecioUnitario { get; init; }
    public decimal Subtotal { get; init; }
    public string? NumeroAutorizacionSri { get; init; }
    public string? SeriePrincipal { get; init; }
    public string? SerieSecundaria { get; init; }
    public int? SecuencialDesde { get; init; }
    public int? SecuencialHasta { get; init; }
    public bool DatosCompletos { get; init; }
}
