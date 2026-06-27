namespace ImprentaSR.Domain.Entities;

public class Categoria : BaseEntity
{
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool Estado { get; private set; } = true;

    private Categoria() { }

    public Categoria(string nombre, string? descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public void Desactivar() => Estado = false;
}
