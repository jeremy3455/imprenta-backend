using ImprentaSR.Domain.Entities;

namespace ImprentaSR.Domain.Interfaces;

public interface IProductoRepository : IRepository<Producto>
{
    Task<(IReadOnlyList<Producto> Items, int TotalCount)> GetFilteredAsync(
        int? categoriaId, bool? esDocumentoTributario, bool? estado, string? search,
        int page, int pageSize);

    Task<IReadOnlyList<Producto>> GetByTipoContribuyenteAsync(string tipoContribuyente);

    Task<IReadOnlyList<Categoria>> GetCategoriasActivasAsync();
}
