using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Infrastructure.Repositories;
using ImprentaSR.WebAPI.Models;
using ImprentaSR.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ImprentaSR.WebAPI.Controllers;

/// <summary>
/// Controlador de autenticación.
/// Provee endpoints para login y registro de usuarios usando la base de datos SQL.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UsuarioRepository _usuarioRepository;
    private readonly OtpService _otpService;
    private readonly IConfiguration _configuration;

    public AuthController(
        UsuarioRepository usuarioRepository,
        OtpService otpService,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _otpService = otpService;
        _configuration = configuration;
    }

    /// <summary>
    /// Valida credenciales y envía un código OTP de 6 dígitos al correo del usuario.
    /// Para correos de prueba el código es el configurado en appsettings (123456).
    /// </summary>
    /// <response code="200">OTP generado y enviado (simulado).</response>
    /// <response code="401">Credenciales inválidas o cuenta bloqueada.</response>
    [HttpPost("request-otp")]
    public async Task<ActionResult<RequestOtpResponse>> RequestOtp([FromBody] LoginRequest request)
    {
        var user = await _usuarioRepository.GetByEmailAsync(request.Email);

        if (user is null)
            return Unauthorized(new { message = "Credenciales inválidas." });

        if (user.BloqueadoHasta.HasValue && user.BloqueadoHasta > DateTime.UtcNow)
            return Unauthorized(new { message = "Cuenta bloqueada. Intenta más tarde." });

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            await _usuarioRepository.RegistrarIntentoFallidoAsync(user.Id);
            return Unauthorized(new { message = "Credenciales inválidas." });
        }

        var (maskedEmail, expiresIn) = _otpService.GenerateAndStore(user.Id, user.Email);

        return Ok(new RequestOtpResponse
        {
            Message = "Código de verificación enviado a tu correo.",
            MaskedEmail = maskedEmail,
            ExpiresInSeconds = expiresIn
        });
    }

    /// <summary>
    /// Verifica el código OTP e inicia sesión devolviendo el token JWT.
    /// </summary>
    /// <response code="200">OTP válido, sesión iniciada.</response>
    /// <response code="400">Código inválido o expirado.</response>
    [HttpPost("verify-otp")]
    public async Task<ActionResult<AuthResponse>> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || request.Code.Trim().Length != 6 || !request.Code.All(char.IsDigit))
            return BadRequest(new { message = "El código debe tener exactamente 6 dígitos." });

        var userId = _otpService.Validate(request.Email, request.Code);
        if (userId is null)
            return BadRequest(new { message = "Código inválido o expirado. Solicita uno nuevo." });

        var user = await _usuarioRepository.GetByIdAsync(userId.Value);
        if (user is null)
            return BadRequest(new { message = "Usuario no encontrado." });

        await _usuarioRepository.RegistrarLoginExitosoAsync(user.Id);

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
    /// Inicia sesión con credenciales válidas y devuelve un token JWT.
    /// Verifica el bloqueo por intentos fallidos y la contraseña con BCrypt.
    /// </summary>
    /// <response code="200">Inicio de sesión exitoso.</response>
    /// <response code="401">Credenciales inválidas o cuenta bloqueada.</response>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _usuarioRepository.GetByEmailAsync(request.Email);

        if (user is null)
            return Unauthorized(new { message = "Credenciales inválidas." });

        // Verificar bloqueo
        if (user.BloqueadoHasta.HasValue && user.BloqueadoHasta > DateTime.UtcNow)
            return Unauthorized(new { message = "Cuenta bloqueada. Intenta más tarde." });

        // Verificar contraseña
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            await _usuarioRepository.RegistrarIntentoFallidoAsync(user.Id);
            return Unauthorized(new { message = "Credenciales inválidas." });
        }

        // Login exitoso
        await _usuarioRepository.RegistrarLoginExitosoAsync(user.Id);

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
    /// <response code="200">Usuario registrado exitosamente.</response>
    /// <response code="400">El correo ya está registrado.</response>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var existing = await _usuarioRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            return BadRequest(new { message = "El correo ya está registrado." });

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var usuario = new Usuario(request.Nombre, request.Email, passwordHash, request.Rol);

        var id = await _usuarioRepository.AddAsync(usuario);
        var created = await _usuarioRepository.GetByIdAsync(id);

        var token = GenerateJwtToken(created!);
        return Ok(new AuthResponse
        {
            Token = token,
            Nombre = created!.Nombre,
            Email = created.Email,
            Rol = created.Rol
        });
    }

    /// <summary>
    /// Genera un token JWT para el usuario especificado.
    /// </summary>
    private string GenerateJwtToken(Usuario user)
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
}
