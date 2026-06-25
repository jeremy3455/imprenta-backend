using ImprentaSR.Domain.Enums;
using ImprentaSR.Domain.ValueObjects;

namespace ImprentaSR.Domain.Entities;

/// <summary>
/// Entidad que representa un cliente de la imprenta.
/// Contiene sus datos de contacto, dirección y estado actual.
/// </summary>
public class Cliente : BaseEntity
{
    /// <summary>Nombre completo del cliente.</summary>
    public string Nombre { get; private set; }

    /// <summary>Correo electrónico del cliente.</summary>
    public string Email { get; private set; }

    /// <summary>Número de teléfono de contacto.</summary>
    public string Telefono { get; private set; }

    /// <summary>Dirección física del cliente (Value Object).</summary>
    public Direccion Direccion { get; private set; }

    /// <summary>Estado actual del cliente (Activo/Inactivo/Suspendido).</summary>
    public ClienteStatus Status { get; private set; }

    /// <summary>Constructor privado requerido por EF Core.</summary>
    private Cliente() { }

    /// <summary>
    /// Crea un nuevo cliente con los datos proporcionados.
    /// El cliente se crea con estado Activo por defecto.
    /// </summary>
    /// <param name="nombre">Nombre completo del cliente.</param>
    /// <param name="email">Correo electrónico.</param>
    /// <param name="telefono">Número de teléfono.</param>
    /// <param name="direccion">Dirección física.</param>
    public Cliente(string nombre, string email, string telefono, Direccion direccion)
    {
        Nombre = nombre;
        Email = email;
        Telefono = telefono;
        Direccion = direccion;
        Status = ClienteStatus.Activo;
    }

    /// <summary>
    /// Actualiza los datos principales del cliente.
    /// </summary>
    /// <param name="nombre">Nuevo nombre.</param>
    /// <param name="email">Nuevo correo electrónico.</param>
    /// <param name="telefono">Nuevo teléfono.</param>
    /// <param name="direccion">Nueva dirección.</param>
    public void UpdateData(string nombre, string email, string telefono, Direccion direccion)
    {
        Nombre = nombre;
        Email = email;
        Telefono = telefono;
        Direccion = direccion;
        SetUpdated();
    }

    /// <summary>
    /// Cambia el estado del cliente.
    /// </summary>
    /// <param name="newStatus">Nuevo estado a asignar.</param>
    public void UpdateStatus(ClienteStatus newStatus)
    {
        Status = newStatus;
        SetUpdated();
    }
}
