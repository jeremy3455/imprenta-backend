namespace ImprentaSR.Application.DTOs;

public record NotificacionDto
{
    public int Id { get; init; }
    public string Mensaje { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int? ReferenciaId { get; init; }
    public bool Leida { get; init; }
    public DateTime Fecha { get; init; }
}
