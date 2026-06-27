using Dapper;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;
using ImprentaSR.Infrastructure.Data;

namespace ImprentaSR.Infrastructure.Repositories;

public class NotificacionRepository : INotificacionRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public NotificacionRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Notificacion>> GetByUsuarioIdAsync(int usuarioId, bool soloNoLeidas = false)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var where = soloNoLeidas ? "AND Leida = 0" : "";
        var sql = $"""
            SELECT * FROM Notificaciones
            WHERE UsuarioId = @UsuarioId {where}
            ORDER BY Fecha DESC
            """;

        var items = (await connection.QueryAsync<Notificacion>(sql, new { UsuarioId = usuarioId }))
            .AsList().AsReadOnly();
        return items;
    }

    public async Task<int> CountNoLeidasAsync(int usuarioId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT COUNT(1) FROM Notificaciones WHERE UsuarioId = @UsuarioId AND Leida = 0";
        return await connection.ExecuteScalarAsync<int>(sql, new { UsuarioId = usuarioId });
    }

    public async Task AddAsync(Notificacion notificacion)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            INSERT INTO Notificaciones (UsuarioId, Mensaje, Tipo, ReferenciaId, Leida, Fecha)
            VALUES (@UsuarioId, @Mensaje, @Tipo, @ReferenciaId, @Leida, @Fecha);
            """;
        await connection.ExecuteAsync(sql, notificacion);
    }

    public async Task MarcarComoLeidaAsync(int id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = "UPDATE Notificaciones SET Leida = 1 WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task MarcarTodasComoLeidasAsync(int usuarioId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = "UPDATE Notificaciones SET Leida = 1 WHERE UsuarioId = @UsuarioId AND Leida = 0";
        await connection.ExecuteAsync(sql, new { UsuarioId = usuarioId });
    }
}
