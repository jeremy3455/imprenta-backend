namespace ImprentaSR.Application.DTOs;

public record ProductoDto
{
    public int Id { get; init; }
    public int CategoriaId { get; init; }
    public string CategoriaNombre { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public decimal PrecioUnitario { get; init; }
    public bool EsDocumentoTributario { get; init; }
    public string? TipoContribuyenteAplicable { get; init; }
    public bool Estado { get; init; }
    public DateTime FechaRegistro { get; init; }
}
