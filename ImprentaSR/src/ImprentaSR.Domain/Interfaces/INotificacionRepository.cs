using ImprentaSR.Domain.Entities;

namespace ImprentaSR.Domain.Interfaces;

public interface INotificacionRepository
{
    Task<IReadOnlyList<Notificacion>> GetByUsuarioIdAsync(int usuarioId, bool soloNoLeidas = false);
    Task<int> CountNoLeidasAsync(int usuarioId);
    Task AddAsync(Notificacion notificacion);
    Task MarcarComoLeidaAsync(int id);
    Task MarcarTodasComoLeidasAsync(int usuarioId);
}
