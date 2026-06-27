using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;

namespace ImprentaSR.Application.UseCases.Pedidos;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProductoRepository _productoRepository;

    public PedidoService(IPedidoRepository pedidoRepository, IProductoRepository productoRepository)
    {
        _pedidoRepository = pedidoRepository;
        _productoRepository = productoRepository;
    }

    public async Task<PagedResult<PedidoResumenDto>> GetAllAsync(PedidoFiltroDto filtro)
    {
        var (items, totalCount) = await _pedidoRepository.GetFilteredAsync(
            filtro.Estado, filtro.ClienteId, filtro.FormaPago,
            filtro.FechaDesde, filtro.FechaHasta, filtro.Search,
            filtro.Page, filtro.PageSize);

        return new PagedResult<PedidoResumenDto>
        {
            Items = items.Select(MapToResumen).ToList().AsReadOnly(),
            TotalCount = totalCount,
            Page = filtro.Page,
            PageSize = filtro.PageSize,
        };
    }

    public async Task<PedidoDetalleDto?> GetByIdAsync(int id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id);
        return pedido is not null ? await MapToDetalle(pedido) : null;
    }

    public async Task<PedidoDetalleDto> CreateAsync(PedidoCreateDto dto)
    {
        // Crear pedido con estado inicial
        var pedido = new Pedido(
            dto.ClienteId, dto.FormaPago, dto.MontoAnticipo,
            dto.FechaVencimientoCredito, dto.Observaciones);

        var pedidoId = await _pedidoRepository.AddAsync(pedido);

        decimal montoTotal = 0;
        bool hayPendientesSri = false;

        foreach (var itemDto in dto.Items)
        {
            var detalle = new DetallePedido(pedidoId, itemDto.ProductoId,
                itemDto.Cantidad, itemDto.PrecioUnitario);

            detalle.CompletarDatosSri(
                itemDto.NumeroAutorizacionSri,
                itemDto.SeriePrincipal,
                itemDto.SerieSecundaria,
                itemDto.SecuencialDesde,
                itemDto.SecuencialHasta);

            await _pedidoRepository.AddDetalleAsync(detalle);
            montoTotal += detalle.Subtotal;

            if (!detalle.DatosCompletos)
            {
                var producto = await _productoRepository.GetByIdAsync(itemDto.ProductoId);
                if (producto?.EsDocumentoTributario == true)
                    hayPendientesSri = true;
            }
        }

        // Actualizar monto total y estado
        pedido = (await _pedidoRepository.GetByIdAsync(pedidoId))!;
        pedido.CalcularMontoTotal(pedido.Detalles);

        if (hayPendientesSri)
            pedido.PonerEnEsperaDatos();

        await _pedidoRepository.UpdateAsync(pedido);

        return await MapToDetalle(pedido);
    }

    public async Task<PedidoDetalleDto> UpdateAsync(int id, PedidoCreateDto dto)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Pedido con Id {id} no encontrado.");

        if (pedido.Estado is not ("Ingresado" or "EnEsperaDatos"))
            throw new InvalidOperationException("Solo se puede editar pedidos en estado Ingresado o EnEsperaDatos.");

        // TODO: implementar edición completa si es necesario
        throw new NotImplementedException("Edición de pedidos no implementada aún.");
    }

    public async Task<PedidoDetalleDto> AprobarAsync(int id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Pedido con Id {id} no encontrado.");

        // Verificar que todos los detalles obligatorios tengan datos SRI
        var detalles = pedido.Detalles;
        foreach (var detalle in detalles)
        {
            if (!detalle.DatosCompletos)
            {
                var producto = await _productoRepository.GetByIdAsync(detalle.ProductoId);
                if (producto?.EsDocumentoTributario == true)
                    throw new InvalidOperationException(
                        "No se puede aprobar el pedido porque hay ítems con datos SRI pendientes.");
            }
        }

        pedido.Aprobar();
        await _pedidoRepository.UpdateAsync(pedido);
        return await MapToDetalle(pedido);
    }

    public async Task<PedidoDetalleDto> IniciarProduccionAsync(int id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Pedido con Id {id} no encontrado.");

        pedido.IniciarProduccion();
        await _pedidoRepository.UpdateAsync(pedido);
        return await MapToDetalle(pedido);
    }

    public async Task<PedidoDetalleDto> MarcarListoEntregaAsync(int id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Pedido con Id {id} no encontrado.");

        pedido.MarcarListoEntrega();
        await _pedidoRepository.UpdateAsync(pedido);
        return await MapToDetalle(pedido);
    }

    public async Task<PedidoDetalleDto> MarcarEntregadoAsync(int id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Pedido con Id {id} no encontrado.");

        pedido.MarcarEntregado();
        await _pedidoRepository.UpdateAsync(pedido);
        return await MapToDetalle(pedido);
    }

    public async Task<PedidoDetalleDto> AnularAsync(int id, string motivo)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Pedido con Id {id} no encontrado.");

        pedido.Anular(motivo);
        await _pedidoRepository.UpdateAsync(pedido);
        return await MapToDetalle(pedido);
    }

    public async Task<PedidoDetalleDto> CompletarDatosSriAsync(int pedidoId, int detalleId, DatosSriDto dto)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(pedidoId)
            ?? throw new KeyNotFoundException($"Pedido con Id {pedidoId} no encontrado.");

        var detalle = pedido.Detalles.FirstOrDefault(d => d.Id == detalleId)
            ?? throw new KeyNotFoundException($"Detalle con Id {detalleId} no encontrado.");

        detalle.CompletarDatosSri(
            dto.NumeroAutorizacionSri,
            dto.SeriePrincipal,
            dto.SerieSecundaria,
            dto.SecuencialDesde,
            dto.SecuencialHasta);

        await _pedidoRepository.UpdateDetalleAsync(detalle);

        // Si todos los detalles están completos, cambiar a Aprobado
        var todosCompletos = pedido.Detalles.All(d => d.DatosCompletos);
        if (todosCompletos && pedido.Estado == "EnEsperaDatos")
        {
            pedido.Aprobar();
            await _pedidoRepository.UpdateAsync(pedido);
        }

        return await MapToDetalle(pedido);
    }

    private static PedidoResumenDto MapToResumen(Pedido p) => new()
    {
        Id = p.Id,
        ClienteId = p.ClienteId,
        RazonSocialCliente = p.Cliente?.RazonSocial ?? string.Empty,
        Estado = p.Estado,
        FormaPago = p.FormaPago,
        MontoTotal = p.MontoTotal,
        MontoPendiente = p.MontoPendiente,
        FechaRegistro = p.FechaRegistro,
        CantidadItems = p.Detalles?.Count ?? 0,
    };

    private async Task<PedidoDetalleDto> MapToDetalle(Pedido p)
    {
        var detalles = p.Detalles.Any()
            ? p.Detalles.ToList().AsReadOnly()
            : await _pedidoRepository.GetDetallesByPedidoIdAsync(p.Id);
        var detalleDtos = new List<DetallePedidoDto>();
        foreach (var d in detalles)
        {
            var producto = d.Producto ?? await _productoRepository.GetByIdAsync(d.ProductoId);
            detalleDtos.Add(new DetallePedidoDto
            {
                Id = d.Id,
                ProductoId = d.ProductoId,
                NombreProducto = producto?.Nombre ?? string.Empty,
                EsDocumentoTributario = producto?.EsDocumentoTributario ?? false,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal,
                NumeroAutorizacionSri = d.NumeroAutorizacionSri,
                SeriePrincipal = d.SeriePrincipal,
                SerieSecundaria = d.SerieSecundaria,
                SecuencialDesde = d.SecuencialDesde,
                SecuencialHasta = d.SecuencialHasta,
                DatosCompletos = d.DatosCompletos,
            });
        }

        return new PedidoDetalleDto
        {
            Id = p.Id,
            ClienteId = p.ClienteId,
            RazonSocialCliente = p.Cliente?.RazonSocial ?? string.Empty,
            NumeroCedulaRuc = p.Cliente?.NumeroCedulaRuc ?? string.Empty,
            Estado = p.Estado,
            FormaPago = p.FormaPago,
            MontoTotal = p.MontoTotal,
            MontoAnticipo = p.MontoAnticipo,
            MontoPendiente = p.MontoPendiente,
            FechaVencimientoCredito = p.FechaVencimientoCredito,
            Observaciones = p.Observaciones,
            FechaRegistro = p.FechaRegistro,
            FechaAprobacion = p.FechaAprobacion,
            FechaInicioProduccion = p.FechaInicioProduccion,
            FechaListoEntrega = p.FechaListoEntrega,
            FechaEntrega = p.FechaEntrega,
            FechaAnulacion = p.FechaAnulacion,
            MotivoAnulacion = p.MotivoAnulacion,
            Detalles = detalleDtos.AsReadOnly(),
        };
    }

}
