namespace ImprentaSR.Domain.Entities;

/// <summary>
/// Clase base abstracta para todas las entidades del dominio.
/// Provee propiedades comunes como Id, fechas de creación/actualización y estado activo.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Identificador único de la entidad (UUID).</summary>
    public Guid Id { get; protected set; }

    /// <summary>Fecha y hora (UTC) en que se creó la entidad.</summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>Fecha y hora (UTC) de la última actualización, o null si nunca se modificó.</summary>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>Indica si la entidad está activa (soft delete).</summary>
    public bool IsActive { get; protected set; } = true;

    /// <summary>
    /// Constructor protegido que asigna un nuevo Id y la fecha de creación automáticamente.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>Marca la entidad como actualizada, asignando la fecha/hora actual a UpdatedAt.</summary>
    public void SetUpdated() => UpdatedAt = DateTime.UtcNow;

    /// <summary>Desactiva la entidad (soft delete).</summary>
    public void Deactivate() => IsActive = false;

    /// <summary>Reactiva la entidad.</summary>
    public void Activate() => IsActive = true;
}
