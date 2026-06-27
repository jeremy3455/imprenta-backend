namespace ImprentaSR.Application.DTOs;

public record DatosSriDto
{
    public string? NumeroAutorizacionSri { get; init; }
    public string? SeriePrincipal { get; init; }
    public string? SerieSecundaria { get; init; }
    public int? SecuencialDesde { get; init; }
    public int? SecuencialHasta { get; init; }
}
