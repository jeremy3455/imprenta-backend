namespace ImprentaSR.Domain.Entities;

public class DetalleSolicitud
{
    public int Id { get; private set; }
    public int SolicitudId { get; private set; }
    public int ProductoId { get; private set; }
    public int Cantidad { get; private set; }
    public string? Observacion { get; private set; }

    public Producto? Producto { get; private set; }

    private DetalleSolicitud() { }

    public DetalleSolicitud(int solicitudId, int productoId, int cantidad, string? observacion)
    {
        SolicitudId = solicitudId;
        ProductoId = productoId;
        Cantidad = cantidad;
        Observacion = observacion;
    }
}
