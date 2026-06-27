namespace ImprentaSR.WebAPI.Services;

/// <summary>
/// Entrada almacenada en caché para validación OTP.
/// </summary>
internal sealed class OtpEntry
{
    /// <summary>Id del usuario asociado al OTP.</summary>
    public int UserId { get; init; }

    /// <summary>Código OTP de 6 dígitos.</summary>
    public string Code { get; init; } = string.Empty;
}
