namespace ImprentaSR.Domain.Entities;

public class Pedido : BaseEntity
{
    public int ClienteId { get; private set; }
    public string Estado { get; private set; } = "Ingresado";
    public string FormaPago { get; private set; } = string.Empty;
    public decimal MontoTotal { get; private set; }
    public decimal MontoAnticipo { get; private set; }
    public decimal MontoPendiente { get; private set; }
    public DateTime? FechaVencimientoCredito { get; private set; }
    public string? Observaciones { get; private set; }
    public DateTime FechaRegistro { get; private set; } = DateTime.Now;
    public DateTime? FechaAprobacion { get; private set; }
    public DateTime? FechaInicioProduccion { get; private set; }
    public DateTime? FechaListoEntrega { get; private set; }
    public DateTime? FechaEntrega { get; private set; }
    public DateTime? FechaAnulacion { get; private set; }
    public string? MotivoAnulacion { get; private set; }

    public Cliente? Cliente { get; set; }
    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();

    private Pedido() { }

    public Pedido(int clienteId, string formaPago, decimal montoAnticipo,
        DateTime? fechaVencimientoCredito, string? observaciones)
    {
        ClienteId = clienteId;
        FormaPago = formaPago;
        MontoAnticipo = montoAnticipo;
        FechaVencimientoCredito = fechaVencimientoCredito;
        Observaciones = observaciones;
        MontoTotal = 0;
    }

    public void CalcularMontoTotal(IEnumerable<DetallePedido> detalles)
    {
        MontoTotal = detalles.Sum(d => d.Subtotal);
    }

    public void PonerEnEsperaDatos()
    {
        if (Estado != "Ingresado")
            throw new InvalidOperationException("Solo se puede poner en espera desde estado Ingresado.");
        Estado = "EnEsperaDatos";
    }

    public void Aprobar()
    {
        Estado = "Aprobado";
        FechaAprobacion = DateTime.Now;
    }

    public void IniciarProduccion()
    {
        if (Estado != "Aprobado")
            throw new InvalidOperationException("Solo se puede iniciar producción desde estado Aprobado.");
        Estado = "EnProduccion";
        FechaInicioProduccion = DateTime.Now;
    }

    public void MarcarListoEntrega()
    {
        if (Estado != "EnProduccion")
            throw new InvalidOperationException("Solo se puede marcar listo desde estado EnProduccion.");
        Estado = "ListoEntrega";
        FechaListoEntrega = DateTime.Now;
    }

    public void MarcarEntregado()
    {
        if (Estado != "ListoEntrega")
            throw new InvalidOperationException("Solo se puede entregar desde estado ListoEntrega.");
        Estado = "Entregado";
        FechaEntrega = DateTime.Now;
    }

    public void Anular(string motivo)
    {
        if (Estado is "EnProduccion" or "ListoEntrega" or "Entregado")
            throw new InvalidOperationException("El pedido no puede anularse porque ya está en producción.");
        Estado = "Anulado";
        FechaAnulacion = DateTime.Now;
        MotivoAnulacion = motivo;
    }
}
