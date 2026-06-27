using ImprentaSR.Domain.Entities;

namespace ImprentaSR.Domain.Interfaces;

public interface IPedidoRepository
{
    Task<(IReadOnlyList<Pedido> Items, int TotalCount)> GetFilteredAsync(
        string? estado, int? clienteId, string? formaPago,
        DateTime? fechaDesde, DateTime? fechaHasta, string? search,
        int page, int pageSize);

    Task<Pedido?> GetByIdAsync(int id);
    Task<int> AddAsync(Pedido pedido);
    Task AddDetalleAsync(DetallePedido detalle);
    Task UpdateAsync(Pedido pedido);
    Task UpdateDetalleAsync(DetallePedido detalle);
    Task<IReadOnlyList<DetallePedido>> GetDetallesByPedidoIdAsync(int pedidoId);
    Task<DetallePedido?> GetDetalleByIdAsync(int detalleId);
}
