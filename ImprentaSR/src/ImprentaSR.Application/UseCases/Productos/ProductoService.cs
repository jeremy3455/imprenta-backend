using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;

namespace ImprentaSR.Application.UseCases.Productos;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;

    public ProductoService(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<PagedResult<ProductoDto>> GetAllAsync(ProductoFiltroDto filtro)
    {
        var (items, totalCount) = await _productoRepository.GetFilteredAsync(
            filtro.CategoriaId, filtro.EsDocumentoTributario, filtro.Estado,
            filtro.Search, filtro.Page, filtro.PageSize);

        return new PagedResult<ProductoDto>
        {
            Items = items.Select(MapToDto).ToList().AsReadOnly(),
            TotalCount = totalCount,
            Page = filtro.Page,
            PageSize = filtro.PageSize,
        };
    }

    public async Task<ProductoDto?> GetByIdAsync(int id)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        return producto is not null ? MapToDto(producto) : null;
    }

    public async Task<IReadOnlyList<ProductoDto>> GetPorTipoContribuyenteAsync(string tipoContribuyente)
    {
        var items = await _productoRepository.GetByTipoContribuyenteAsync(tipoContribuyente);
        return items.Select(MapToDto).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<CategoriaDto>> GetCategoriasAsync()
    {
        var items = await _productoRepository.GetCategoriasActivasAsync();
        return items.Select(c => new CategoriaDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Descripcion = c.Descripcion,
            Estado = c.Estado,
        }).ToList().AsReadOnly();
    }

    public async Task<ProductoDto> CreateAsync(ProductoCreateDto dto)
    {
        var producto = new Producto(
            dto.CategoriaId, dto.Nombre, dto.Descripcion,
            dto.PrecioUnitario, dto.EsDocumentoTributario,
            dto.TipoContribuyenteAplicable);

        var id = await _productoRepository.AddAsync(producto);
        var created = await _productoRepository.GetByIdAsync(id);
        return MapToDto(created!);
    }

    public async Task<ProductoDto> UpdateAsync(int id, ProductoCreateDto dto)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto is null)
            throw new KeyNotFoundException($"Producto con Id {id} no encontrado.");

        producto.Update(
            dto.CategoriaId, dto.Nombre, dto.Descripcion,
            dto.PrecioUnitario, dto.EsDocumentoTributario,
            dto.TipoContribuyenteAplicable);

        await _productoRepository.UpdateAsync(producto);
        return MapToDto(producto);
    }

    public async Task DeleteAsync(int id)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto is null)
            throw new KeyNotFoundException($"Producto con Id {id} no encontrado.");

        await _productoRepository.DeleteAsync(id);
    }

    private static ProductoDto MapToDto(Producto p) => new()
    {
        Id = p.Id,
        CategoriaId = p.CategoriaId,
        CategoriaNombre = p.Categoria?.Nombre ?? string.Empty,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        PrecioUnitario = p.PrecioUnitario,
        EsDocumentoTributario = p.EsDocumentoTributario,
        TipoContribuyenteAplicable = p.TipoContribuyenteAplicable,
        Estado = p.Estado,
        FechaRegistro = p.FechaRegistro,
    };
}
