using ImprentaSR.Domain.Entities;

namespace ImprentaSR.Domain.Interfaces;

public interface ISolicitudRepository
{
    Task<(IReadOnlyList<Solicitud> Items, int TotalCount)> GetFilteredAsync(
        int? clienteId, string? estado, int page, int pageSize);
    Task<Solicitud?> GetByIdAsync(int id);
    Task<int> AddAsync(Solicitud solicitud);
    Task AddDetalleAsync(DetalleSolicitud detalle);
    Task UpdateAsync(Solicitud solicitud);
}
