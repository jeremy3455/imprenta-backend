namespace ImprentaSR.Domain.Entities;

public class Cliente : BaseEntity
{
    public string NumeroCedulaRuc { get; private set; } = string.Empty;
    public string RazonSocial { get; private set; } = string.Empty;
    public string? Direccion { get; private set; }
    public string? Email { get; private set; }
    public string? Telefono { get; private set; }
    public string? TipoContribuyente { get; private set; }
    public bool Estado { get; private set; } = true;
    public DateTime FechaRegistro { get; private set; } = DateTime.Now;

    private Cliente() { }

    public Cliente(string numeroCedulaRuc, string razonSocial, string? direccion,
        string? email, string? telefono, string? tipoContribuyente)
    {
        NumeroCedulaRuc = numeroCedulaRuc;
        RazonSocial = razonSocial;
        Direccion = direccion;
        Email = email;
        Telefono = telefono;
        TipoContribuyente = tipoContribuyente;
    }

    public void Update(string numeroCedulaRuc, string razonSocial, string? direccion,
        string? email, string? telefono, string? tipoContribuyente)
    {
        NumeroCedulaRuc = numeroCedulaRuc;
        RazonSocial = razonSocial;
        Direccion = direccion;
        Email = email;
        Telefono = telefono;
        TipoContribuyente = tipoContribuyente;
    }

    public void Desactivar() => Estado = false;
    public void Activar() => Estado = true;
}
