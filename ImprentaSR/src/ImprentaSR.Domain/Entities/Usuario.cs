namespace ImprentaSR.Domain.Entities;

/// <summary>
/// Entidad que representa un usuario del sistema.
/// Corresponde a la tabla Usuarios de la base de datos SQL.
/// </summary>
public class Usuario : BaseEntity
{
    /// <summary>Id del cliente asociado (NULL = usuario interno).</summary>
    public int? ClienteId { get; private set; }

    /// <summary>Nombre completo del usuario.</summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>Correo electrónico (único).</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Hash de la contraseña (BCrypt).</summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>Rol del usuario: Admin, Operador, Cliente.</summary>
    public string Rol { get; private set; } = "Cliente";

    /// <summary>Indica si el usuario está activo.</summary>
    public bool Activo { get; private set; } = true;

    /// <summary>Indica si debe cambiar la contraseña en el próximo inicio de sesión.</summary>
    public bool MustChangePassword { get; private set; }

    /// <summary>Indica si el correo electrónico ha sido verificado.</summary>
    public bool EmailVerificado { get; private set; }

    /// <summary>Fecha del último inicio de sesión exitoso.</summary>
    public DateTime? UltimoLogin { get; private set; }

    /// <summary>Número de intentos fallidos de inicio de sesión consecutivos.</summary>
    public int LoginIntentosFallidos { get; private set; }

    /// <summary>Fecha hasta la cual el usuario está bloqueado.</summary>
    public DateTime? BloqueadoHasta { get; private set; }

    /// <summary>Constructor privado requerido por Dapper.</summary>
    private Usuario() { }

    /// <summary>
    /// Crea un nuevo usuario.
    /// </summary>
    /// <param name="nombre">Nombre completo.</param>
    /// <param name="email">Correo electrónico.</param>
    /// <param name="passwordHash">Hash de la contraseña.</param>
    /// <param name="rol">Rol del usuario.</param>
    /// <param name="clienteId">Id del cliente asociado (opcional).</param>
    public Usuario(string nombre, string email, string passwordHash, string rol, int? clienteId = null)
    {
        Nombre = nombre;
        Email = email;
        PasswordHash = passwordHash;
        Rol = rol;
        ClienteId = clienteId;
    }

    /// <summary>Registra un inicio de sesión exitoso.</summary>
    public void RegistrarLogin()
    {
        UltimoLogin = DateTime.UtcNow;
        LoginIntentosFallidos = 0;
    }

    /// <summary>Incrementa el contador de intentos fallidos.</summary>
    public void IncrementarIntentoFallido()
    {
        LoginIntentosFallidos++;
        if (LoginIntentosFallidos >= 5)
            BloqueadoHasta = DateTime.UtcNow.AddMinutes(15);
    }

    /// <summary>Verifica el correo electrónico del usuario.</summary>
    public void VerificarEmail() => EmailVerificado = true;

    /// <summary>Actualiza el hash de la contraseña.</summary>
    public void ActualizarPassword(string nuevoHash) => PasswordHash = nuevoHash;

    /// <summary>Desactiva el usuario.</summary>
    public void Desactivar() => Activo = false;
}
