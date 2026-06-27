namespace ImprentaSR.Domain.Entities;

public class DetallePedido : BaseEntity
{
    public int PedidoId { get; private set; }
    public int ProductoId { get; private set; }
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal Subtotal { get; private set; }
    public string? NumeroAutorizacionSri { get; private set; }
    public string? SeriePrincipal { get; private set; }
    public string? SerieSecundaria { get; private set; }
    public int? SecuencialDesde { get; private set; }
    public int? SecuencialHasta { get; private set; }
    public bool DatosCompletos { get; private set; }

    public Pedido? Pedido { get; set; }
    public Producto? Producto { get; set; }

    private DetallePedido() { }

    public DetallePedido(int pedidoId, int productoId, int cantidad, decimal precioUnitario)
    {
        PedidoId = pedidoId;
        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }

    public void CompletarDatosSri(string? numeroAutorizacion, string? seriePrincipal,
        string? serieSecundaria, int? secuencialDesde, int? secuencialHasta)
    {
        NumeroAutorizacionSri = numeroAutorizacion;
        SeriePrincipal = seriePrincipal;
        SerieSecundaria = serieSecundaria;
        SecuencialDesde = secuencialDesde;
        SecuencialHasta = secuencialHasta;
        DatosCompletos = !string.IsNullOrWhiteSpace(numeroAutorizacion);
    }
}
