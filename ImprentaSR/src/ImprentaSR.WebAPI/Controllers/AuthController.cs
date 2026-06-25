using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ImprentaSR.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ImprentaSR.WebAPI.Controllers;

/// <summary>
/// Controlador de autenticación.
/// Provee endpoints para login, registro y verificación de sesión.
/// Los usuarios se almacenan en memoria mientras no se implemente la base de datos SQL.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private static readonly List<UserStore> Users = [];
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Inicializa el controlador con usuarios por defecto.
    /// </summary>
    /// <param name="configuration">Configuración de la aplicación (JWT).</param>
    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;

        if (Users.Count == 0)
        {
            Users.Add(new UserStore
            {
                Id = 1,
                Nombre = "Admin",
                Email = "admin@imprenta.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Rol = "Admin"
            });
            Users.Add(new UserStore
            {
                Id = 2,
                Nombre = "Operador",
                Email = "operador@imprenta.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Operador123!"),
                Rol = "Operador"
            });
        }
    }

    /// <summary>
    /// Inicia sesión con credenciales válidas y devuelve un token JWT.
    /// </summary>
    /// <param name="request">Credenciales de acceso.</param>
    /// <returns>Token JWT y datos del usuario.</returns>
    /// <response code="200">Inicio de sesión exitoso.</response>
    /// <response code="401">Credenciales inválidas.</response>
    [HttpPost("login")]
    public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
    {
        var user = Users.FirstOrDefault(u =>
            u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Credenciales inválidas." });
        }

        var token = GenerateJwtToken(user);
        return Ok(new AuthResponse
        {
            Token = token,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        });
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="request">Datos del nuevo usuario.</param>
    /// <returns>Token JWT y datos del usuario creado.</returns>
    /// <response code="200">Usuario registrado exitosamente.</response>
    /// <response code="400">El correo ya está registrado.</response>
    [HttpPost("register")]
    public ActionResult<AuthResponse> Register([FromBody] RegisterRequest request)
    {
        if (Users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return BadRequest(new { message = "El correo ya está registrado." });
        }

        var newId = Users.Count > 0 ? Users.Max(u => u.Id) + 1 : 1;
        var user = new UserStore
        {
            Id = newId,
            Nombre = request.Nombre,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Rol = request.Rol
        };
        Users.Add(user);

        var token = GenerateJwtToken(user);
        return Ok(new AuthResponse
        {
            Token = token,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        });
    }

    /// <summary>
    /// Genera un token JWT para el usuario especificado.
    /// </summary>
    /// <param name="user">Datos del usuario.</param>
    /// <returns>Token JWT en formato string.</returns>
    private string GenerateJwtToken(UserStore user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Nombre),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Rol)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Almacenamiento en memoria para usuarios (temporal hasta implementar SQL Server).
    /// </summary>
    private class UserStore
    {
        public int Id { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string PasswordHash { get; init; } = string.Empty;
        public string Rol { get; init; } = "Cliente";
    }
}
