using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;

namespace ImprentaSR.Application.UseCases.Notificaciones;

public class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _notificacionRepository;

    public NotificacionService(INotificacionRepository notificacionRepository)
    {
        _notificacionRepository = notificacionRepository;
    }

    public async Task<IReadOnlyList<NotificacionDto>> GetByUsuarioAsync(int usuarioId)
    {
        var items = await _notificacionRepository.GetByUsuarioIdAsync(usuarioId);
        return items.Select(MapToDto).ToList().AsReadOnly();
    }

    public async Task<int> CountNoLeidasAsync(int usuarioId)
    {
        return await _notificacionRepository.CountNoLeidasAsync(usuarioId);
    }

    public async Task CreateAsync(int usuarioId, string mensaje, string tipo, int? referenciaId = null)
    {
        var notificacion = new Notificacion(usuarioId, mensaje, tipo, referenciaId);
        await _notificacionRepository.AddAsync(notificacion);
    }

    public async Task MarcarComoLeidaAsync(int id)
    {
        await _notificacionRepository.MarcarComoLeidaAsync(id);
    }

    public async Task MarcarTodasComoLeidasAsync(int usuarioId)
    {
        await _notificacionRepository.MarcarTodasComoLeidasAsync(usuarioId);
    }

    private static NotificacionDto MapToDto(Notificacion n) => new()
    {
        Id = n.Id,
        Mensaje = n.Mensaje,
        Tipo = n.Tipo,
        ReferenciaId = n.ReferenciaId,
        Leida = n.Leida,
        Fecha = n.Fecha,
    };
}
