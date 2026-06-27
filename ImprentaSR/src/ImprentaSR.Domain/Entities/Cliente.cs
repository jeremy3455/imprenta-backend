namespace ImprentaSR.Domain.Entities;

/// <summary>
/// Entidad que representa un cliente de la imprenta.
/// Corresponde a la tabla Clientes de la base de datos SQL.
/// </summary>
public class Cliente : BaseEntity
{
    /// <summary>RUC del cliente (13 dígitos, único).</summary>
    public string Ruc { get; private set; } = string.Empty;

    /// <summary>Razón social o nombre completo.</summary>
    public string RazonSocial { get; private set; } = string.Empty;

    /// <summary>Dirección física.</summary>
    public string? Direccion { get; private set; }

    /// <summary>Teléfono de contacto.</summary>
    public string? Telefono { get; private set; }

    /// <summary>Correo electrónico.</summary>
    public string? Email { get; private set; }

    /// <summary>Indica si el cliente está activo.</summary>
    public bool Activo { get; private set; } = true;

    /// <summary>Constructor privado requerido por Dapper.</summary>
    private Cliente() { }

    /// <summary>
    /// Crea un nuevo cliente.
    /// </summary>
    /// <param name="ruc">RUC del cliente (13 dígitos).</param>
    /// <param name="razonSocial">Razón social.</param>
    /// <param name="direccion">Dirección física (opcional).</param>
    /// <param name="telefono">Teléfono (opcional).</param>
    /// <param name="email">Correo electrónico (opcional).</param>
    public Cliente(string ruc, string razonSocial, string? direccion, string? telefono, string? email)
    {
        Ruc = ruc;
        RazonSocial = razonSocial;
        Direccion = direccion;
        Telefono = telefono;
        Email = email;
    }

    /// <summary>
    /// Actualiza los datos del cliente.
    /// </summary>
    public void Update(string ruc, string razonSocial, string? direccion, string? telefono, string? email)
    {
        Ruc = ruc;
        RazonSocial = razonSocial;
        Direccion = direccion;
        Telefono = telefono;
        Email = email;
    }

    /// <summary>Desactiva el cliente.</summary>
    public void Desactivar() => Activo = false;

    /// <summary>Reactiva el cliente.</summary>
    public void Activar() => Activo = true;
}
