namespace ImprentaSR.Domain.Entities;

/// <summary>
/// Clase base abstracta para todas las entidades del dominio.
/// Provee propiedades comunes: Id autoincremental (IDENTITY) y fecha de creación.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Identificador único de la entidad (autoincremental - IDENTITY).</summary>
    public int Id { get; protected set; }

    /// <summary>Fecha y hora (UTC) en que se creó la entidad.</summary>
    public DateTime CreadoEn { get; protected set; } = DateTime.UtcNow;
}
