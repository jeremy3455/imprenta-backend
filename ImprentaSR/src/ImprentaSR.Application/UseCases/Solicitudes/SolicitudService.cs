using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;

namespace ImprentaSR.Application.UseCases.Solicitudes;

public class SolicitudService : ISolicitudService
{
    private readonly ISolicitudRepository _solicitudRepository;
    private readonly IProductoRepository _productoRepository;

    public SolicitudService(
        ISolicitudRepository solicitudRepository,
        IProductoRepository productoRepository)
    {
        _solicitudRepository = solicitudRepository;
        _productoRepository = productoRepository;
    }

    public async Task<PagedResult<SolicitudResumenDto>> GetAllAsync(
        int? clienteId, string? estado, int page, int pageSize)
    {
        var (items, totalCount) = await _solicitudRepository.GetFilteredAsync(
            clienteId, estado, page, pageSize);

        return new PagedResult<SolicitudResumenDto>
        {
            Items = items.Select(MapToResumen).ToList().AsReadOnly(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<SolicitudDetalleDto?> GetByIdAsync(int id)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(id);
        return solicitud is not null ? MapToDetalle(solicitud) : null;
    }

    public async Task<SolicitudDetalleDto> CreateAsync(int clienteId, SolicitudCreateDto dto)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            throw new ArgumentException("Debe agregar al menos un producto.");

        var formaPago = dto.FormaPago ?? "EFECTIVO";
        var solicitud = new Solicitud(clienteId, formaPago, dto.Observacion);
        var solicitudId = await _solicitudRepository.AddAsync(solicitud);

        foreach (var item in dto.Items)
        {
            var producto = await _productoRepository.GetByIdAsync(item.ProductoId)
                ?? throw new ArgumentException($"Producto con Id {item.ProductoId} no encontrado.");

            var detalle = new DetalleSolicitud(solicitudId, item.ProductoId, item.Cantidad, item.Observacion);
            await _solicitudRepository.AddDetalleAsync(detalle);
        }

        var created = await _solicitudRepository.GetByIdAsync(solicitudId);
        return MapToDetalle(created!);
    }

    public async Task<SolicitudDetalleDto> AprobarAsync(int id)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Solicitud con Id {id} no encontrada.");

        solicitud.Aprobar();
        await _solicitudRepository.UpdateAsync(solicitud);
        return MapToDetalle(solicitud);
    }

    public async Task<SolicitudDetalleDto> VincularPedidoAsync(int id, int pedidoId, decimal montoTotal)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Solicitud con Id {id} no encontrada.");

        solicitud.Aprobar();
        solicitud.VincularPedido(pedidoId, montoTotal);
        await _solicitudRepository.UpdateAsync(solicitud);
        return MapToDetalle(solicitud);
    }

    public async Task<SolicitudDetalleDto> RechazarAsync(int id)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Solicitud con Id {id} no encontrada.");

        solicitud.Rechazar();
        await _solicitudRepository.UpdateAsync(solicitud);
        return MapToDetalle(solicitud);
    }

    private static SolicitudResumenDto MapToResumen(Solicitud s) => new()
    {
        Id = s.Id,
        ClienteId = s.ClienteId,
        RazonSocialCliente = s.Cliente?.RazonSocial ?? string.Empty,
        Estado = s.Estado,
        Observacion = s.Observacion,
        FechaSolicitud = s.FechaSolicitud,
        CantidadItems = s.Detalles?.Count ?? 0,
    };

    private static SolicitudDetalleDto MapToDetalle(Solicitud s) => new()
    {
        Id = s.Id,
        ClienteId = s.ClienteId,
        RazonSocialCliente = s.Cliente?.RazonSocial ?? string.Empty,
        NumeroCedulaRuc = s.Cliente?.NumeroCedulaRuc ?? string.Empty,
        Estado = s.Estado,
        FormaPago = s.FormaPago,
        PedidoId = s.PedidoId,
        MontoTotal = s.MontoTotal,
        Observacion = s.Observacion,
        FechaSolicitud = s.FechaSolicitud,
        Items = s.Detalles?.Select(d => new SolicitudDetalleItemDto
        {
            Id = d.Id,
            ProductoId = d.ProductoId,
            NombreProducto = d.Producto?.Nombre ?? string.Empty,
            Cantidad = d.Cantidad,
            Observacion = d.Observacion,
        }).ToList().AsReadOnly() ?? new List<SolicitudDetalleItemDto>().AsReadOnly(),
    };
}
