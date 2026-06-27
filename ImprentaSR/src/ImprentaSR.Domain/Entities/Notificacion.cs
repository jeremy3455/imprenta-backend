namespace ImprentaSR.Domain.Entities;

public class Notificacion
{
    public int Id { get; private set; }
    public int UsuarioId { get; private set; }
    public string Mensaje { get; private set; } = string.Empty;
    public string Tipo { get; private set; } = "General";
    public int? ReferenciaId { get; private set; }
    public bool Leida { get; private set; }
    public DateTime Fecha { get; private set; } = DateTime.UtcNow;

    private Notificacion() { }

    public Notificacion(int usuarioId, string mensaje, string tipo, int? referenciaId = null)
    {
        UsuarioId = usuarioId;
        Mensaje = mensaje;
        Tipo = tipo;
        ReferenciaId = referenciaId;
    }

    public void MarcarComoLeida() => Leida = true;
}
