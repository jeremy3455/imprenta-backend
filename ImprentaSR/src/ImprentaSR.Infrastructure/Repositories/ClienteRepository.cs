using Dapper;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;
using ImprentaSR.Infrastructure.Data;

namespace ImprentaSR.Infrastructure.Repositories;

/// <summary>
/// Repositorio Dapper para la entidad Cliente (tabla Clientes).
/// Implementa operaciones CRUD usando SQL puro.
/// </summary>
public class ClienteRepository : IRepository<Cliente>
{
    private readonly DbConnectionFactory _connectionFactory;

    public ClienteRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Obtiene un cliente por su Id.
    /// </summary>
    public async Task<Cliente?> GetByIdAsync(int id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Cliente>(
            "SELECT * FROM Clientes WHERE Id = @Id", new { Id = id });
    }

    /// <summary>
    /// Obtiene todos los clientes activos.
    /// </summary>
    public async Task<IReadOnlyList<Cliente>> GetAllAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var clientes = await connection.QueryAsync<Cliente>(
            "SELECT * FROM Clientes WHERE Activo = 1 ORDER BY RazonSocial");
        return clientes.ToList().AsReadOnly();
    }

    /// <summary>
    /// Crea un nuevo cliente y retorna el Id generado (IDENTITY).
    /// </summary>
    public async Task<int> AddAsync(Cliente entity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            INSERT INTO Clientes (Ruc, RazonSocial, Direccion, Telefono, Email)
            VALUES (@Ruc, @RazonSocial, @Direccion, @Telefono, @Email);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    /// <summary>
    /// Actualiza los datos de un cliente existente.
    /// </summary>
    public async Task UpdateAsync(Cliente entity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            UPDATE Clientes
            SET Ruc = @Ruc, RazonSocial = @RazonSocial, Direccion = @Direccion,
                Telefono = @Telefono, Email = @Email
            WHERE Id = @Id
            """;
        await connection.ExecuteAsync(sql, entity);
    }

    /// <summary>
    /// Desactiva (soft delete) un cliente por su Id.
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync(
            "UPDATE Clientes SET Activo = 0 WHERE Id = @Id", new { Id = id });
    }

    /// <summary>
    /// Cuenta los clientes activos.
    /// </summary>
    public async Task<int> CountAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Clientes WHERE Activo = 1");
    }
}
