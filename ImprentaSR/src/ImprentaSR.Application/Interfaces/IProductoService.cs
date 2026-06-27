using ImprentaSR.Application.DTOs;

namespace ImprentaSR.Application.Interfaces;

public interface IProductoService
{
    Task<PagedResult<ProductoDto>> GetAllAsync(ProductoFiltroDto filtro);
    Task<ProductoDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<ProductoDto>> GetPorTipoContribuyenteAsync(string tipoContribuyente);
    Task<IReadOnlyList<CategoriaDto>> GetCategoriasAsync();
    Task<ProductoDto> CreateAsync(ProductoCreateDto dto);
    Task<ProductoDto> UpdateAsync(int id, ProductoCreateDto dto);
    Task DeleteAsync(int id);
}
