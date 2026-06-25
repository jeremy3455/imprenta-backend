namespace ImprentaSR.Domain.Enums;

/// <summary>
/// Estados posibles para un cliente dentro del sistema.
/// </summary>
public enum ClienteStatus
{
    /// <summary>Cliente activo con capacidad de realizar pedidos.</summary>
    Activo = 1,

    /// <summary>Cliente dado de baja temporal.</summary>
    Inactivo = 2,

    /// <summary>Cliente suspendido por incumplimiento.</summary>
    Suspendido = 3
}
