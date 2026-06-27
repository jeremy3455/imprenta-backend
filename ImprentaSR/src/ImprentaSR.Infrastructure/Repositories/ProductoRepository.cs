using Dapper;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;
using ImprentaSR.Infrastructure.Data;

namespace ImprentaSR.Infrastructure.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public ProductoRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Producto>> GetAllAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var lookup = new Dictionary<int, Producto>();

        await connection.QueryAsync<Producto, Categoria, Producto>(@"
            SELECT p.Id, p.Nombre, p.Descripcion, p.PrecioUnitario, p.CategoriaId, p.EsDocumentoTributario, p.TipoContribuyenteAplicable, p.Estado, p.FechaRegistro, c.Id AS CatId, c.Nombre, c.Descripcion, c.Estado
            FROM Productos p
            INNER JOIN Categorias c ON c.Id = p.CategoriaId
            WHERE p.Estado = 1
            ORDER BY p.Nombre",
            (producto, categoria) =>
            {
                if (producto is null) return producto!;
                if (!lookup.TryGetValue(producto.Id, out var existing))
                {
                    lookup.Add(producto.Id, producto);
                    existing = producto;
                }
                if (categoria is not null)
                    existing.Categoria = categoria;
                return existing;
            }, splitOn: "CatId");

        return lookup.Values.ToList().AsReadOnly();
    }

    public async Task<Producto?> GetByIdAsync(int id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return (await connection.QueryAsync<Producto, Categoria, Producto>(@"
            SELECT p.Id, p.Nombre, p.Descripcion, p.PrecioUnitario, p.CategoriaId, p.EsDocumentoTributario, p.TipoContribuyenteAplicable, p.Estado, p.FechaRegistro, c.Id AS CatId, c.Nombre, c.Descripcion, c.Estado
            FROM Productos p
            INNER JOIN Categorias c ON c.Id = p.CategoriaId
            WHERE p.Id = @Id",
            (producto, categoria) =>
            {
                if (producto is not null && categoria is not null)
                    producto.Categoria = categoria;
                return producto!;
            }, new { Id = id }, splitOn: "CatId"))
            .FirstOrDefault();
    }

    public async Task<(IReadOnlyList<Producto> Items, int TotalCount)> GetFilteredAsync(
        int? categoriaId, bool? esDocumentoTributario, bool? estado, string? search,
        int page, int pageSize)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var where = new List<string>();
        var parameters = new DynamicParameters();

        if (categoriaId.HasValue)
        {
            where.Add("p.CategoriaId = @CatId");
            parameters.Add("CatId", categoriaId.Value);
        }
        if (esDocumentoTributario.HasValue)
        {
            where.Add("p.EsDocumentoTributario = @EsDocTrib");
            parameters.Add("EsDocTrib", esDocumentoTributario.Value);
        }
        if (estado.HasValue)
        {
            where.Add("p.Estado = @Estado");
            parameters.Add("Estado", estado.Value);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            where.Add("p.Nombre LIKE @Search");
            parameters.Add("Search", $"%{search}%");
        }

        var whereClause = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";

        var countSql = $"SELECT COUNT(1) FROM Productos p {whereClause}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        parameters.Add("Offset", (page - 1) * pageSize);
        parameters.Add("PageSize", pageSize);

        var querySql = $@"
            SELECT p.Id, p.Nombre, p.Descripcion, p.PrecioUnitario, p.CategoriaId, p.EsDocumentoTributario, p.TipoContribuyenteAplicable, p.Estado, p.FechaRegistro, c.Id AS CatId, c.Nombre, c.Descripcion, c.Estado
            FROM Productos p
            INNER JOIN Categorias c ON c.Id = p.CategoriaId
            {whereClause}
            ORDER BY p.Nombre
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var lookup = new Dictionary<int, Producto>();
        await connection.QueryAsync<Producto, Categoria, Producto>(querySql,
            (producto, categoria) =>
            {
                if (producto is null) return producto!;
                if (!lookup.TryGetValue(producto.Id, out var existing))
                {
                    lookup.Add(producto.Id, producto);
                    existing = producto;
                }
                if (categoria is not null)
                    existing.Categoria = categoria;
                return existing;
            }, parameters, splitOn: "CatId");

        return (lookup.Values.ToList().AsReadOnly(), totalCount);
    }

    public async Task<IReadOnlyList<Producto>> GetByTipoContribuyenteAsync(string tipoContribuyente)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var lookup = new Dictionary<int, Producto>();

        await connection.QueryAsync<Producto, Categoria, Producto>(@"
            SELECT p.Id, p.Nombre, p.Descripcion, p.PrecioUnitario, p.CategoriaId, p.EsDocumentoTributario, p.TipoContribuyenteAplicable, p.Estado, p.FechaRegistro, c.Id AS CatId, c.Nombre, c.Descripcion, c.Estado
            FROM Productos p
            INNER JOIN Categorias c ON c.Id = p.CategoriaId
            WHERE p.Estado = 1
              AND (p.TipoContribuyenteAplicable IS NULL
                   OR p.TipoContribuyenteAplicable = @Tipo)
            ORDER BY p.Nombre",
            (producto, categoria) =>
            {
                if (producto is null) return producto!;
                if (!lookup.TryGetValue(producto.Id, out var existing))
                {
                    lookup.Add(producto.Id, producto);
                    existing = producto;
                }
                if (categoria is not null)
                    existing.Categoria = categoria;
                return existing;
            }, new { Tipo = tipoContribuyente }, splitOn: "CatId");

        return lookup.Values.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Categoria>> GetCategoriasActivasAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return (await connection.QueryAsync<Categoria>(
            "SELECT * FROM Categorias WHERE Estado = 1 ORDER BY Nombre"))
            .ToList().AsReadOnly();
    }

    public async Task<int> AddAsync(Producto entity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            INSERT INTO Productos (CategoriaId, Nombre, Descripcion, PrecioUnitario,
                EsDocumentoTributario, TipoContribuyenteAplicable)
            VALUES (@CategoriaId, @Nombre, @Descripcion, @PrecioUnitario,
                @EsDocumentoTributario, @TipoContribuyenteAplicable);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task UpdateAsync(Producto entity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            UPDATE Productos
            SET CategoriaId = @CategoriaId, Nombre = @Nombre, Descripcion = @Descripcion,
                PrecioUnitario = @PrecioUnitario,
                EsDocumentoTributario = @EsDocumentoTributario,
                TipoContribuyenteAplicable = @TipoContribuyenteAplicable
            WHERE Id = @Id
            """;
        await connection.ExecuteAsync(sql, entity);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync(
            "UPDATE Productos SET Estado = 0 WHERE Id = @Id", new { Id = id });
    }

    public async Task<int> CountAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Productos WHERE Estado = 1");
    }
}
