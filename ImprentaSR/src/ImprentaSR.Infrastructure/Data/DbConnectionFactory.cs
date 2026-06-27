using System.Data;
using Microsoft.Data.SqlClient;

namespace ImprentaSR.Infrastructure.Data;

/// <summary>
/// Fábrica de conexiones a SQL Server.
/// Crea y administra conexiones usando la cadena de conexión configurada.
/// </summary>
public class DbConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Inicializa la fábrica con la cadena de conexión.
    /// </summary>
    /// <param name="connectionString">Cadena de conexión a SQL Server.</param>
    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Crea una nueva conexión SQL abierta.
    /// </summary>
    /// <returns>Conexión abierta a la base de datos.</returns>
    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
