using Dapper;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Infrastructure.Data;

namespace ImprentaSR.Infrastructure.Repositories;

/// <summary>
/// Repositorio Dapper para la entidad Usuario (tabla Usuarios).
/// Provee métodos específicos de autenticación además del CRUD básico.
/// </summary>
public class UsuarioRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public UsuarioRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Obtiene un usuario por su correo electrónico.
    /// </summary>
    /// <param name="email">Correo electrónico.</param>
    /// <returns>El usuario encontrado o null.</returns>
    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Usuario>(
            "SELECT * FROM Usuarios WHERE Email = @Email AND Activo = 1",
            new { Email = email });
    }

    /// <summary>
    /// Obtiene un usuario por su Id.
    /// </summary>
    public async Task<Usuario?> GetByIdAsync(int id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Usuario>(
            "SELECT * FROM Usuarios WHERE Id = @Id", new { Id = id });
    }

    /// <summary>
    /// Crea un nuevo usuario y retorna el Id generado.
    /// </summary>
    public async Task<int> AddAsync(Usuario entity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            INSERT INTO Usuarios (ClienteId, Nombre, Email, PasswordHash, Rol)
            VALUES (@ClienteId, @Nombre, @Email, @PasswordHash, @Rol);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    /// <summary>
    /// Actualiza los datos de un usuario.
    /// </summary>
    public async Task UpdateAsync(Usuario entity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            UPDATE Usuarios
            SET Nombre = @Nombre, Email = @Email, PasswordHash = @PasswordHash,
                Rol = @Rol, UltimoLogin = @UltimoLogin,
                LoginIntentosFallidos = @LoginIntentosFallidos,
                BloqueadoHasta = @BloqueadoHasta
            WHERE Id = @Id
            """;
        await connection.ExecuteAsync(sql, entity);
    }

    /// <summary>
    /// Registra un intento de login fallido para un usuario.
    /// </summary>
    public async Task RegistrarIntentoFallidoAsync(int usuarioId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            UPDATE Usuarios
            SET LoginIntentosFallidos = LoginIntentosFallidos + 1,
                BloqueadoHasta = CASE
                    WHEN LoginIntentosFallidos >= 4 THEN DATEADD(MINUTE, 15, GETUTCDATE())
                    ELSE BloqueadoHasta
                END
            WHERE Id = @Id
            """;
        await connection.ExecuteAsync(sql, new { Id = usuarioId });
    }

    /// <summary>
    /// Obtiene todos los usuarios con rol Admin u Operador.
    /// </summary>
    public async Task<IReadOnlyList<Usuario>> GetAllAdminsAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var items = await connection.QueryAsync<Usuario>(
            "SELECT * FROM Usuarios WHERE Rol IN ('Admin', 'Operador') AND Activo = 1");
        return items.ToList().AsReadOnly();
    }

    /// <summary>
    /// Registra un login exitoso (resetea intentos fallidos).
    /// </summary>
    public async Task RegistrarLoginExitosoAsync(int usuarioId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = """
            UPDATE Usuarios
            SET UltimoLogin = GETUTCDATE(), LoginIntentosFallidos = 0
            WHERE Id = @Id
            """;
        await connection.ExecuteAsync(sql, new { Id = usuarioId });
    }
}
