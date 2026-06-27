using Dapper;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;
using ImprentaSR.Infrastructure.Data;

namespace ImprentaSR.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public PedidoRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<(IReadOnlyList<Pedido> Items, int TotalCount)> GetFilteredAsync(
        string? estado, int? clienteId, string? formaPago,
        DateTime? fechaDesde, DateTime? fechaHasta, string? search,
        int page, int pageSize)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var where = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(estado))
        {
            where.Add("p.Estado = @Estado");
            parameters.Add("Estado", estado);
        }
        if (clienteId.HasValue)
        {
            where.Add("p.ClienteId = @ClienteId");
            parameters.Add("ClienteId", clienteId.Value);
        }
        if (!string.IsNullOrWhiteSpace(formaPago))
        {
            where.Add("p.FormaPago = @FormaPago");
            parameters.Add("FormaPago", formaPago);
        }
        if (fechaDesde.HasValue)
        {
            where.Add("p.FechaRegistro >= @FechaDesde");
            parameters.Add("FechaDesde", fechaDesde.Value);
        }
        if (fechaHasta.HasValue)
        {
            where.Add("p.FechaRegistro <= @FechaHasta");
            parameters.Add("FechaHasta", fechaHasta.Value);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            where.Add("c.RazonSocial LIKE @Search");
            parameters.Add("Search", $"%{search}%");
        }

        var whereClause = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";

        var countSql = $"SELECT COUNT(1) FROM Pedidos p INNER JOIN Clientes c ON c.Id = p.ClienteId {whereClause}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        parameters.Add("Offset", (page - 1) * pageSize);
        parameters.Add("PageSize", pageSize);

        var querySql = $@"
            SELECT p.*, c.Id, c.NumeroCedulaRuc, c.RazonSocial, c.Direccion, c.Email, c.Telefono, c.TipoContribuyente, c.Estado, c.FechaRegistro
            FROM Pedidos p
            INNER JOIN Clientes c ON c.Id = p.ClienteId
            {whereClause}
            ORDER BY p.FechaRegistro DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var lookup = new Dictionary<int, Pedido>();
    await connection.QueryAsync<Pedido, Cliente, Pedido>(querySql,
            (pedido, cliente) =>
            {
                if (!lookup.TryGetValue(pedido.Id, out var existing))
                {
                    lookup.Add(pedido.Id, pedido);
                    existing = pedido;
                }
                existing.Cliente = cliente;
                return existing;
            }, parameters, splitOn: "Id");

        // Cargar detalles para cada pedido
        var ids = lookup.Keys.ToList();
        if (ids.Count > 0)
        {
            var detalles = await connection.QueryAsync<DetallePedido>(
                "SELECT * FROM DetallePedido WHERE PedidoId IN @Ids", new { Ids = ids });

            foreach (var detalle in detalles)
            {
                if (lookup.TryGetValue(detalle.PedidoId, out var pedido))
                    pedido.Detalles.Add(detalle);
            }
        }

        return (lookup.Values.ToList().AsReadOnly(), totalCount);
    }

    public async Task<Pedido?> GetByIdAsync(int id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var pedido = (await connection.QueryAsync<Pedido, Cliente, Pedido>(@"
            SELECT p.*, c.Id, c.NumeroCedulaRuc, c.RazonSocial, c.Direccion, c.Email, c.Telefono, c.TipoContribuyente, c.Estado, c.FechaRegistro
            FROM Pedidos p
            INNER JOIN Clientes c ON c.Id = p.ClienteId
            WHERE p.Id = @Id",
            (p, c) =>
            {
                p.Cliente = c;
                return p;
            }, new { Id = id }, splitOn: "Id"))
            .FirstOrDefault();

        if (pedido is not null)
        {
            var detalles = await connection.QueryAsync<DetallePedido>(
                "SELECT * FROM DetallePedido WHERE PedidoId = @Id", new { Id = id });
            foreach (var d in detalles)
                pedido.Detalles.Add(d);
        }

        return pedido;
    }

    public async Task<int> AddAsync(Pedido pedido)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            INSERT INTO Pedidos (ClienteId, Estado, FormaPago, MontoTotal, MontoAnticipo,
                FechaVencimientoCredito, Observaciones)
            VALUES (@ClienteId, @Estado, @FormaPago, @MontoTotal, @MontoAnticipo,
                @FechaVencimientoCredito, @Observaciones);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;
        return await connection.ExecuteScalarAsync<int>(sql, pedido);
    }

    public async Task AddDetalleAsync(DetallePedido detalle)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            INSERT INTO DetallePedido (PedidoId, ProductoId, Cantidad, PrecioUnitario,
                NumeroAutorizacionSri, SeriePrincipal, SerieSecundaria,
                SecuencialDesde, SecuencialHasta, DatosCompletos)
            VALUES (@PedidoId, @ProductoId, @Cantidad, @PrecioUnitario,
                @NumeroAutorizacionSri, @SeriePrincipal, @SerieSecundaria,
                @SecuencialDesde, @SecuencialHasta, @DatosCompletos);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;
        var id = await connection.ExecuteScalarAsync<int>(sql, detalle);
        detalle.GetType().GetProperty("Id")?.SetValue(detalle, id);
    }

    public async Task UpdateAsync(Pedido pedido)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            UPDATE Pedidos
            SET Estado = @Estado, MontoTotal = @MontoTotal, MontoAnticipo = @MontoAnticipo,
                FechaVencimientoCredito = @FechaVencimientoCredito,
                Observaciones = @Observaciones,
                FechaAprobacion = @FechaAprobacion,
                FechaInicioProduccion = @FechaInicioProduccion,
                FechaListoEntrega = @FechaListoEntrega,
                FechaEntrega = @FechaEntrega,
                FechaAnulacion = @FechaAnulacion,
                MotivoAnulacion = @MotivoAnulacion
            WHERE Id = @Id
            """;
        await connection.ExecuteAsync(sql, pedido);
    }

    public async Task UpdateDetalleAsync(DetallePedido detalle)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            UPDATE DetallePedido
            SET NumeroAutorizacionSri = @NumeroAutorizacionSri,
                SeriePrincipal = @SeriePrincipal,
                SerieSecundaria = @SerieSecundaria,
                SecuencialDesde = @SecuencialDesde,
                SecuencialHasta = @SecuencialHasta,
                DatosCompletos = @DatosCompletos
            WHERE Id = @Id
            """;
        await connection.ExecuteAsync(sql, detalle);
    }

    public async Task<IReadOnlyList<DetallePedido>> GetDetallesByPedidoIdAsync(int pedidoId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var result = await connection.QueryAsync<DetallePedido>(
            "SELECT * FROM DetallePedido WHERE PedidoId = @PedidoId", new { PedidoId = pedidoId });
        return result.ToList().AsReadOnly();
    }

    public async Task<DetallePedido?> GetDetalleByIdAsync(int detalleId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<DetallePedido>(
            "SELECT * FROM DetallePedido WHERE Id = @Id", new { Id = detalleId });
    }
}
