namespace ImprentaSR.Application.DTOs;

public record ClienteDto
{
    public int Id { get; init; }
    public string NumeroCedulaRuc { get; init; } = string.Empty;
    public string RazonSocial { get; init; } = string.Empty;
    public string? Direccion { get; init; }
    public string? Email { get; init; }
    public string? Telefono { get; init; }
    public string? TipoContribuyente { get; init; }
    public bool Estado { get; init; }
    public DateTime FechaRegistro { get; init; }
}

public record CreateClienteDto
{
    public string NumeroCedulaRuc { get; init; } = string.Empty;
    public string RazonSocial { get; init; } = string.Empty;
    public string? Direccion { get; init; }
    public string? Email { get; init; }
    public string? Telefono { get; init; }
    public string? TipoContribuyente { get; init; }
}

public record UpdateClienteDto
{
    public string NumeroCedulaRuc { get; init; } = string.Empty;
    public string RazonSocial { get; init; } = string.Empty;
    public string? Direccion { get; init; }
    public string? Email { get; init; }
    public string? Telefono { get; init; }
    public string? TipoContribuyente { get; init; }
}

public record SriRespuestaDto
{
    public string NumeroCedulaRuc { get; init; } = string.Empty;
    public string RazonSocial { get; init; } = string.Empty;
    public string? Direccion { get; init; }
    public string? TipoContribuyente { get; init; }
    public string? EstadoContribuyente { get; init; }
    public bool Encontrado { get; init; }
}
