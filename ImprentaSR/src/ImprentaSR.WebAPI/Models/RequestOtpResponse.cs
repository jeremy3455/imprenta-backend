namespace ImprentaSR.WebAPI.Models;

/// <summary>
/// Respuesta al solicitar un código OTP tras validar credenciales.
/// </summary>
public record RequestOtpResponse
{
    /// <summary>Mensaje informativo para el usuario.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>Correo enmascarado al que se envió el código.</summary>
    public string MaskedEmail { get; init; } = string.Empty;

    /// <summary>Segundos hasta que expire el código.</summary>
    public int ExpiresInSeconds { get; init; }
}
