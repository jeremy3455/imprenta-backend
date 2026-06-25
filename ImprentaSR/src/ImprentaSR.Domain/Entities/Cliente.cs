using ImprentaSR.Domain.Enums;
using ImprentaSR.Domain.ValueObjects;

namespace ImprentaSR.Domain.Entities;

public class Cliente : BaseEntity
{
    public string Nombre { get; private set; }
    public string Email { get; private set; }
    public string Telefono { get; private set; }
    public Direccion Direccion { get; private set; }
    public ClienteStatus Status { get; private set; }

    private Cliente() { } // EF Core

    public Cliente(string nombre, string email, string telefono, Direccion direccion)
    {
        Nombre = nombre;
        Email = email;
        Telefono = telefono;
        Direccion = direccion;
        Status = ClienteStatus.Activo;
    }

    public void UpdateData(string nombre, string email, string telefono, Direccion direccion)
    {
        Nombre = nombre;
        Email = email;
        Telefono = telefono;
        Direccion = direccion;
        SetUpdated();
    }

    public void UpdateStatus(ClienteStatus newStatus)
    {
        Status = newStatus;
        SetUpdated();
    }
}
