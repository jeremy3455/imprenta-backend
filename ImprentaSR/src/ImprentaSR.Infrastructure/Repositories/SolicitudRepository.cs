using Dapper;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;
using ImprentaSR.Infrastructure.Data;

namespace ImprentaSR.Infrastructure.Repositories;

public class SolicitudRepository : ISolicitudRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public SolicitudRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<(IReadOnlyList<Solicitud> Items, int TotalCount)> GetFilteredAsync(
        int? clienteId, string? estado, int page, int pageSize)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var where = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (clienteId.HasValue)
        {
            where.Add("s.ClienteId = @ClienteId");
            parameters.Add("ClienteId", clienteId.Value);
        }
        if (!string.IsNullOrWhiteSpace(estado))
        {
            where.Add("s.Estado = @Estado");
            parameters.Add("Estado", estado);
        }

        var whereClause = string.Join(" AND ", where);

        var countSql = $"SELECT COUNT(1) FROM Solicitudes s WHERE {whereClause}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var offset = (page - 1) * pageSize;
        var sql = $"""
            SELECT s.*, c.Id, c.RazonSocial, c.NumeroCedulaRuc
            FROM Solicitudes s
            INNER JOIN Clientes c ON c.Id = s.ClienteId
            WHERE {whereClause}
            ORDER BY s.FechaSolicitud DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

        var items = (await connection.QueryAsync<Solicitud, Cliente, Solicitud>(
            sql,
            (solicitud, cliente) =>
            {
                solicitud.GetType().GetProperty("Cliente")?.SetValue(solicitud, cliente);
                return solicitud;
            },
            new { Offset = offset, PageSize = pageSize, clienteId, estado },
            splitOn: "Id"
        )).AsList().AsReadOnly();

        return (items, totalCount);
    }

    public async Task<Solicitud?> GetByIdAsync(int id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql = """
            SELECT s.*, c.Id, c.RazonSocial, c.NumeroCedulaRuc
            FROM Solicitudes s
            INNER JOIN Clientes c ON c.Id = s.ClienteId
            WHERE s.Id = @Id
            """;

        var solicitud = await connection.QueryAsync<Solicitud, Cliente, Solicitud>(
            sql,
            (s, c) =>
            {
                s.GetType().GetProperty("Cliente")?.SetValue(s, c);
                return s;
            },
            new { Id = id },
            splitOn: "Id"
        );

        var result = solicitud.FirstOrDefault();
        if (result is null) return null;

        var detalleSql = """
            SELECT d.*, p.Id, p.Nombre
            FROM DetalleSolicitud d
            INNER JOIN Productos p ON p.Id = d.ProductoId
            WHERE d.SolicitudId = @SolicitudId
            """;

        var detalles = await connection.QueryAsync<DetalleSolicitud, Producto, DetalleSolicitud>(
            detalleSql,
            (d, p) =>
            {
                d.GetType().GetProperty("Producto")?.SetValue(d, p);
                return d;
            },
            new { SolicitudId = id },
            splitOn: "Id"
        );

        result.GetType().GetProperty("Detalles")?.SetValue(result, detalles.ToList());

        return result;
    }

    public async Task<int> AddAsync(Solicitud solicitud)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            INSERT INTO Solicitudes (ClienteId, Estado, FormaPago, Observacion, FechaSolicitud)
            VALUES (@ClienteId, @Estado, @FormaPago, @Observacion, @FechaSolicitud);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;
        return await connection.ExecuteScalarAsync<int>(sql, solicitud);
    }

    public async Task AddDetalleAsync(DetalleSolicitud detalle)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            INSERT INTO DetalleSolicitud (SolicitudId, ProductoId, Cantidad, Observacion)
            VALUES (@SolicitudId, @ProductoId, @Cantidad, @Observacion);
            """;
        await connection.ExecuteAsync(sql, detalle);
    }

    public async Task UpdateAsync(Solicitud solicitud)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            UPDATE Solicitudes
            SET Estado = @Estado,
                FormaPago = @FormaPago,
                PedidoId = @PedidoId,
                MontoTotal = @MontoTotal
            WHERE Id = @Id
            """;
        await connection.ExecuteAsync(sql, new { solicitud.Estado, solicitud.FormaPago, solicitud.PedidoId, solicitud.MontoTotal, solicitud.Id });
    }
}
