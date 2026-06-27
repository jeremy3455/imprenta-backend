namespace ImprentaSR.Domain.Entities;

public class Solicitud : BaseEntity
{
    public int ClienteId { get; private set; }
    public string Estado { get; private set; } = "Pendiente";
    public string FormaPago { get; private set; } = "EFECTIVO";
    public int? PedidoId { get; private set; }
    public decimal? MontoTotal { get; private set; }
    public string? Observacion { get; private set; }
    public DateTime FechaSolicitud { get; private set; } = DateTime.UtcNow;

    public Cliente? Cliente { get; private set; }
    public List<DetalleSolicitud> Detalles { get; private set; } = new();

    private Solicitud() { }

    public Solicitud(int clienteId, string formaPago, string? observacion)
    {
        ClienteId = clienteId;
        FormaPago = formaPago;
        Observacion = observacion;
    }

    public void AgregarDetalle(int productoId, int cantidad, string? observacion)
    {
        Detalles.Add(new DetalleSolicitud(Id, productoId, cantidad, observacion));
    }

    public void Aprobar()
    {
        if (Estado != "Pendiente")
            throw new InvalidOperationException("Solo se puede aprobar solicitudes en estado Pendiente.");
        Estado = "Aprobada";
    }

    public void Rechazar()
    {
        if (Estado != "Pendiente")
            throw new InvalidOperationException("Solo se puede rechazar solicitudes en estado Pendiente.");
        Estado = "Rechazada";
    }

    public void VincularPedido(int pedidoId, decimal montoTotal)
    {
        PedidoId = pedidoId;
        MontoTotal = montoTotal;
    }
}
