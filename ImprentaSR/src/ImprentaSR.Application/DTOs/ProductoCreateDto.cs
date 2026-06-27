namespace ImprentaSR.Application.DTOs;

public record ProductoCreateDto
{
    public int CategoriaId { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public decimal PrecioUnitario { get; init; }
    public bool EsDocumentoTributario { get; init; }
    public string? TipoContribuyenteAplicable { get; init; }
}
