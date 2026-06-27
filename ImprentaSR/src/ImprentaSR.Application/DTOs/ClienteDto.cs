namespace ImprentaSR.Application.DTOs;

/// <summary>
/// DTO de salida con los datos completos de un cliente.
/// Corresponde a la tabla Clientes de la base de datos.
/// </summary>
public record ClienteDto
{
    /// <summary>Identificador único del cliente (IDENTITY).</summary>
    public int Id { get; init; }

    /// <summary>RUC del cliente (13 dígitos).</summary>
    public string Ruc { get; init; } = string.Empty;

    /// <summary>Razón social.</summary>
    public string RazonSocial { get; init; } = string.Empty;

    /// <summary>Dirección física.</summary>
    public string? Direccion { get; init; }

    /// <summary>Teléfono de contacto.</summary>
    public string? Telefono { get; init; }

    /// <summary>Correo electrónico.</summary>
    public string? Email { get; init; }

    /// <summary>Indica si el cliente está activo.</summary>
    public bool Activo { get; init; }
}

/// <summary>
/// DTO de entrada para crear un nuevo cliente.
/// </summary>
public record CreateClienteDto
{
    /// <summary>RUC del cliente (13 dígitos).</summary>
    public string Ruc { get; init; } = string.Empty;

    /// <summary>Razón social.</summary>
    public string RazonSocial { get; init; } = string.Empty;

    /// <summary>Dirección física.</summary>
    public string? Direccion { get; init; }

    /// <summary>Teléfono de contacto.</summary>
    public string? Telefono { get; init; }

    /// <summary>Correo electrónico.</summary>
    public string? Email { get; init; }
}

/// <summary>
/// DTO de entrada para actualizar un cliente existente.
/// </summary>
public record UpdateClienteDto
{
    /// <summary>RUC del cliente (13 dígitos).</summary>
    public string Ruc { get; init; } = string.Empty;

    /// <summary>Razón social.</summary>
    public string RazonSocial { get; init; } = string.Empty;

    /// <summary>Dirección física.</summary>
    public string? Direccion { get; init; }

    /// <summary>Teléfono de contacto.</summary>
    public string? Telefono { get; init; }

    /// <summary>Correo electrónico.</summary>
    public string? Email { get; init; }
}
