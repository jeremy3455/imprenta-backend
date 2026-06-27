namespace ImprentaSR.Domain.Entities;

public class Producto : BaseEntity
{
    public int CategoriaId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public bool EsDocumentoTributario { get; private set; }
    public string? TipoContribuyenteAplicable { get; private set; }
    public bool Estado { get; private set; } = true;
    public DateTime FechaRegistro { get; private set; } = DateTime.Now;

    public Categoria? Categoria { get; set; }

    private Producto() { }

    public Producto(int categoriaId, string nombre, string? descripcion,
        decimal precioUnitario, bool esDocumentoTributario,
        string? tipoContribuyenteAplicable)
    {
        CategoriaId = categoriaId;
        Nombre = nombre;
        Descripcion = descripcion;
        PrecioUnitario = precioUnitario;
        EsDocumentoTributario = esDocumentoTributario;
        TipoContribuyenteAplicable = tipoContribuyenteAplicable;
    }

    public void Update(int categoriaId, string nombre, string? descripcion,
        decimal precioUnitario, bool esDocumentoTributario,
        string? tipoContribuyenteAplicable)
    {
        CategoriaId = categoriaId;
        Nombre = nombre;
        Descripcion = descripcion;
        PrecioUnitario = precioUnitario;
        EsDocumentoTributario = esDocumentoTributario;
        TipoContribuyenteAplicable = tipoContribuyenteAplicable;
    }

    public void Desactivar() => Estado = false;
    public void Activar() => Estado = true;
}
