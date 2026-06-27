using Microsoft.Extensions.Caching.Memory;

namespace ImprentaSR.WebAPI.Services;

/// <summary>
/// Servicio de generación y validación de códigos OTP en memoria.
/// Para correos de prueba usa un código fijo configurado en appsettings.
/// </summary>
public class OtpService
{
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OtpService> _logger;

    private const string CacheKeyPrefix = "otp:";

    public OtpService(
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<OtpService> logger)
    {
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Genera un OTP de 6 dígitos, lo almacena y simula el envío por correo.
    /// </summary>
    /// <param name="userId">Id del usuario autenticado por credenciales</param>
    /// <param name="email">Correo del destinatario</param>
    /// <returns>Correo enmascarado y segundos de expiración</returns>
    public (string MaskedEmail, int ExpiresInSeconds) GenerateAndStore(int userId, string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var code = ResolveOtpCode(normalizedEmail);
        var expirationMinutes = _configuration.GetValue("Otp:ExpirationMinutes", 5);
        var expiresInSeconds = expirationMinutes * 60;

        var entry = new OtpEntry { UserId = userId, Code = code };
        _cache.Set(
            CacheKeyPrefix + normalizedEmail,
            entry,
            TimeSpan.FromSeconds(expiresInSeconds));

        _logger.LogInformation(
            "Código OTP enviado a {Email} (simulado). Código: {Code}",
            normalizedEmail,
            code);

        return (MaskEmail(normalizedEmail), expiresInSeconds);
    }

    /// <summary>
    /// Valida el código OTP para un correo dado.
    /// </summary>
    /// <param name="email">Correo del usuario</param>
    /// <param name="code">Código ingresado</param>
    /// <returns>Id del usuario si el OTP es válido; null si no</returns>
    public int? Validate(string email, string code)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var key = CacheKeyPrefix + normalizedEmail;

        if (!_cache.TryGetValue(key, out OtpEntry? entry) || entry is null)
            return null;

        if (!string.Equals(entry.Code, code.Trim(), StringComparison.Ordinal))
            return null;

        _cache.Remove(key);
        return entry.UserId;
    }

    /// <summary>
    /// Resuelve el código OTP: fijo para correos de prueba, aleatorio para el resto.
    /// </summary>
    private string ResolveOtpCode(string normalizedEmail)
    {
        var testEmails = _configuration
            .GetSection("Otp:TestEmails")
            .Get<string[]>() ?? [];

        var isTestEmail = testEmails
            .Any(e => string.Equals(e.Trim().ToLowerInvariant(), normalizedEmail, StringComparison.Ordinal));

        if (isTestEmail)
            return _configuration["Otp:TestCode"] ?? "123456";

        return Random.Shared.Next(100000, 999999).ToString();
    }

    /// <summary>
    /// Enmascara el correo para mostrarlo en la UI (ej: a***@imprenta.com).
    /// </summary>
    private static string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2 || parts[0].Length == 0)
            return email;

        var visible = parts[0][0];
        return $"{visible}***@{parts[1]}";
    }
}
