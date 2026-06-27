using ImprentaSR.Application.DTOs;

namespace ImprentaSR.Application.Interfaces;

public interface IPedidoService
{
    Task<PagedResult<PedidoResumenDto>> GetAllAsync(PedidoFiltroDto filtro);
    Task<PedidoDetalleDto?> GetByIdAsync(int id);
    Task<PedidoDetalleDto> CreateAsync(PedidoCreateDto dto);
    Task<PedidoDetalleDto> UpdateAsync(int id, PedidoCreateDto dto);
    Task<PedidoDetalleDto> AprobarAsync(int id);
    Task<PedidoDetalleDto> IniciarProduccionAsync(int id);
    Task<PedidoDetalleDto> MarcarListoEntregaAsync(int id);
    Task<PedidoDetalleDto> MarcarEntregadoAsync(int id);
    Task<PedidoDetalleDto> AnularAsync(int id, string motivo);
    Task<PedidoDetalleDto> CompletarDatosSriAsync(int pedidoId, int detalleId, DatosSriDto dto);
}
