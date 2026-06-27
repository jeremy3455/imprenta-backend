namespace ImprentaSR.Application.DTOs;

public record CategoriaDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public bool Estado { get; init; }
}
