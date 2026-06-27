using ImprentaSR.Application.DTOs;

namespace ImprentaSR.Application.Interfaces;

public interface INotificacionService
{
    Task<IReadOnlyList<NotificacionDto>> GetByUsuarioAsync(int usuarioId);
    Task<int> CountNoLeidasAsync(int usuarioId);
    Task CreateAsync(int usuarioId, string mensaje, string tipo, int? referenciaId = null);
    Task MarcarComoLeidaAsync(int id);
    Task MarcarTodasComoLeidasAsync(int usuarioId);
}
