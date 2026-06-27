using ImprentaSR.Application.DTOs;

namespace ImprentaSR.Application.Interfaces;

public interface ISolicitudService
{
    Task<PagedResult<SolicitudResumenDto>> GetAllAsync(int? clienteId, string? estado, int page, int pageSize);
    Task<SolicitudDetalleDto?> GetByIdAsync(int id);
    Task<SolicitudDetalleDto> CreateAsync(int clienteId, SolicitudCreateDto dto);
    Task<SolicitudDetalleDto> AprobarAsync(int id);
    Task<SolicitudDetalleDto> VincularPedidoAsync(int id, int pedidoId, decimal montoTotal);
    Task<SolicitudDetalleDto> RechazarAsync(int id);
}
