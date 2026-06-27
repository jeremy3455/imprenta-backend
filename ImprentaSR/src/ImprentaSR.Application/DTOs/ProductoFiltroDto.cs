namespace ImprentaSR.Application.DTOs;

public record ProductoFiltroDto
{
    public int? CategoriaId { get; init; }
    public bool? EsDocumentoTributario { get; init; }
    public bool? Estado { get; init; }
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
