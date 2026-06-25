namespace ImprentaSR.Application.DTOs;

/// <summary>
/// DTO de salida con los datos completos de un cliente.
/// Se usa para responder en los endpoints GET, POST y PUT.
/// </summary>
public record ClienteDto
{
    /// <summary>Identificador único del cliente.</summary>
    public Guid Id { get; init; }

    /// <summary>Nombre completo.</summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>Correo electrónico.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Teléfono de contacto.</summary>
    public string Telefono { get; init; } = string.Empty;

    /// <summary>Dirección completa en formato texto.</summary>
    public string Direccion { get; init; } = string.Empty;

    /// <summary>Estado actual del cliente (Activo, Inactivo, Suspendido).</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>Fecha de creación del registro.</summary>
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO de entrada para crear un nuevo cliente.
/// </summary>
public record CreateClienteDto
{
    /// <summary>Nombre completo del cliente.</summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>Correo electrónico.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Teléfono de contacto.</summary>
    public string Telefono { get; init; } = string.Empty;

    /// <summary>Calle y número del domicilio.</summary>
    public string Calle { get; init; } = string.Empty;

    /// <summary>Ciudad.</summary>
    public string Ciudad { get; init; } = string.Empty;

    /// <summary>Estado o provincia.</summary>
    public string Estado { get; init; } = string.Empty;

    /// <summary>Código postal.</summary>
    public string CodigoPostal { get; init; } = string.Empty;
}

/// <summary>
/// DTO de entrada para actualizar los datos de un cliente existente.
/// </summary>
public record UpdateClienteDto
{
    /// <summary>Nombre completo actualizado.</summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>Correo electrónico actualizado.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Teléfono actualizado.</summary>
    public string Telefono { get; init; } = string.Empty;

    /// <summary>Calle y número actualizado.</summary>
    public string Calle { get; init; } = string.Empty;

    /// <summary>Ciudad actualizada.</summary>
    public string Ciudad { get; init; } = string.Empty;

    /// <summary>Estado o provincia actualizado.</summary>
    public string Estado { get; init; } = string.Empty;

    /// <summary>Código postal actualizado.</summary>
    public string CodigoPostal { get; init; } = string.Empty;
}
