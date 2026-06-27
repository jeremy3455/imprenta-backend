using Dapper;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;
using ImprentaSR.Infrastructure.Data;

namespace ImprentaSR.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public ClienteRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Cliente>(
            "SELECT * FROM Clientes WHERE Id = @Id", new { Id = id });
    }

    public async Task<IReadOnlyList<Cliente>> GetAllAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var clientes = await connection.QueryAsync<Cliente>(
            "SELECT * FROM Clientes WHERE Estado = 1 ORDER BY RazonSocial");
        return clientes.ToList().AsReadOnly();
    }

    public async Task<int> AddAsync(Cliente entity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            INSERT INTO Clientes (NumeroCedulaRuc, RazonSocial, Direccion, Email, Telefono, TipoContribuyente)
            VALUES (@NumeroCedulaRuc, @RazonSocial, @Direccion, @Email, @Telefono, @TipoContribuyente);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task UpdateAsync(Cliente entity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            UPDATE Clientes
            SET NumeroCedulaRuc = @NumeroCedulaRuc, RazonSocial = @RazonSocial,
                Direccion = @Direccion, Email = @Email, Telefono = @Telefono,
                TipoContribuyente = @TipoContribuyente
            WHERE Id = @Id
            """;
        await connection.ExecuteAsync(sql, entity);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync(
            "UPDATE Clientes SET Estado = 0 WHERE Id = @Id", new { Id = id });
    }

    public async Task<bool> ExistsByNumeroCedulaRucAsync(string numero)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT COUNT(1) FROM Clientes WHERE NumeroCedulaRuc = @Numero", new { Numero = numero });
    }

    public async Task<int> CountAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Clientes WHERE Estado = 1");
    }
}
