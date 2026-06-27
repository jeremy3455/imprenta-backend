using System.Data;
using Dapper;

namespace ImprentaSR.Infrastructure.Data;

/// <summary>
/// Inicializador de la base de datos.
/// Ejecuta los scripts SQL de schema, seeds y stored procedures
/// para crear la estructura de la base de datos desde cero.
/// </summary>
public class DatabaseInitializer
{
    private readonly DbConnectionFactory _connectionFactory;
    private static readonly string ScriptsBasePath;

    /// <summary>
    /// Constructor estático que resuelve la ruta base de los scripts SQL.
    /// Busca en: directorio del proyecto, solución y compilación.
    /// </summary>
    static DatabaseInitializer()
    {
        // Posibles ubicaciones de la carpeta database/
        var possiblePaths = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "database"),  // desde WebAPI hacia solución
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "database"), // desde src/WebAPI
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database"),
            Path.Combine(Directory.GetCurrentDirectory(), "database"),
        };

        ScriptsBasePath = possiblePaths.FirstOrDefault(Directory.Exists)
            ?? Directory.GetCurrentDirectory(); // fallback
    }

    /// <summary>
    /// Constructor que recibe la fábrica de conexiones.
    /// </summary>
    public DatabaseInitializer(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Inicializa la base de datos ejecutando todos los scripts SQL necesarios.
    /// </summary>
    public async Task InitializeAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        // Crear base de datos si no existe
        await CreateDatabaseIfNotExistsAsync(connection);

        // Cambiar al contexto de ImprentaDB
        await connection.ExecuteAsync("USE ImprentaDB;");

        if (!await IsSchemaInitializedAsync(connection))
        {
            // Ejecutar scripts en orden (00_create_database ya se aplicó arriba)
            await ExecuteScriptAsync(connection, "schema\\01_tables.sql");
            await ExecuteScriptAsync(connection, "schema\\02_indexes.sql");
            await ExecuteScriptAsync(connection, "seeds\\tipos_documento.sql");
            await ExecuteScriptAsync(connection, "procedures\\sp_ObtenerSiguienteSecuencia.sql");
            await ExecuteScriptAsync(connection, "procedures\\sp_ReporteProduccionMensual.sql");
            await ExecuteScriptAsync(connection, "procedures\\sp_MarcarEnviadoSri.sql");
        }

        // Seed usuarios por defecto si no existen
        await SeedDefaultUsersAsync(connection);
    }

    /// <summary>
    /// Verifica si el esquema ya fue aplicado comprobando la tabla Usuarios.
    /// </summary>
    private static async Task<bool> IsSchemaInitializedAsync(IDbConnection connection)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Usuarios';
            """;

        return await connection.ExecuteScalarAsync<int>(sql) > 0;
    }

    /// <summary>
    /// Crea la base de datos ImprentaDB si no existe.
    /// </summary>
    private static async Task CreateDatabaseIfNotExistsAsync(IDbConnection connection)
    {
        var sql = "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ImprentaDB') " +
                  "CREATE DATABASE ImprentaDB;";
        await connection.ExecuteAsync(sql);
    }

    /// <summary>
    /// Ejecuta un script SQL desde un archivo.
    /// </summary>
    private async Task ExecuteScriptAsync(IDbConnection connection, string relativePath)
    {
        var fullPath = Path.Combine(ScriptsBasePath, relativePath);
        if (!File.Exists(fullPath))
            return; // Saltar si el archivo no existe

        var script = await File.ReadAllTextAsync(fullPath);
        if (string.IsNullOrWhiteSpace(script))
            return;

        // Dividir por GO y ejecutar cada lote
        var batches = script.Split(new[] { "\nGO", "\nGO\n", "GO\n", "GO\r\n", "\r\nGO\r\n" },
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var batch in batches)
        {
            var trimmed = batch.Trim()
                .Replace("USE ImprentaDB;", "")
                .Replace("USE ImprentaDB", "");

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                await connection.ExecuteAsync(trimmed);
            }
        }
    }

    /// <summary>
    /// Inserta usuarios por defecto si la tabla está vacía.
    /// </summary>
    private static async Task SeedDefaultUsersAsync(IDbConnection connection)
    {
        var count = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Usuarios");
        if (count > 0) return;

        var adminHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        var operadorHash = BCrypt.Net.BCrypt.HashPassword("Operador123!");

        const string sql = """
            INSERT INTO Usuarios (Nombre, Email, PasswordHash, Rol, EmailVerificado)
            VALUES (@Nombre, @Email, @PasswordHash, @Rol, 1);
            """;

        await connection.ExecuteAsync(sql, new[]
        {
            new { Nombre = "Admin", Email = "admin@imprenta.com", PasswordHash = adminHash, Rol = "Admin" },
            new { Nombre = "Operador", Email = "operador@imprenta.com", PasswordHash = operadorHash, Rol = "Operador" }
        });
    }
}
