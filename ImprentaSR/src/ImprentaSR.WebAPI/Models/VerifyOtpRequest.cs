namespace ImprentaSR.WebAPI.Models;

/// <summary>
/// DTO para verificar el código OTP y completar el inicio de sesión.
/// </summary>
public record VerifyOtpRequest
{
    /// <summary>Correo electrónico del usuario.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Código OTP de 6 dígitos.</summary>
    public string Code { get; init; } = string.Empty;
}
