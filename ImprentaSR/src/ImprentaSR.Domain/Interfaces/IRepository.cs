using ImprentaSR.Domain.Entities;

namespace ImprentaSR.Domain.Interfaces;

/// <summary>
/// Interfaz genérica del patrón Repository para operaciones CRUD básicas.
/// Trabaja únicamente con entidades que heredan de <see cref="BaseEntity"/>.
/// </summary>
/// <typeparam name="T">Tipo de entidad del dominio.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>Obtiene una entidad por su Id.</summary>
    /// <param name="id">Identificador único.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>La entidad encontrada o null si no existe.</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Obtiene todas las entidades activas.</summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de entidades activas.</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Agrega una nueva entidad al repositorio.</summary>
    /// <param name="entity">Entidad a crear.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>La entidad creada con su Id asignado.</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Actualiza una entidad existente.</summary>
    /// <param name="entity">Entidad con los datos modificados.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Elimina (soft delete) una entidad del repositorio.</summary>
    /// <param name="entity">Entidad a desactivar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Cuenta la cantidad de entidades activas.</summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Número total de entidades activas.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
