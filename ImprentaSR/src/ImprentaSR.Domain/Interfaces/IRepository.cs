using ImprentaSR.Domain.Entities;

namespace ImprentaSR.Domain.Interfaces;

/// <summary>
/// Interfaz genérica del patrón Repository para operaciones CRUD básicas.
/// Trabaja con entidades que heredan de <see cref="BaseEntity"/>.
/// </summary>
/// <typeparam name="T">Tipo de entidad del dominio.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>Obtiene una entidad por su Id.</summary>
    /// <param name="id">Identificador único (int).</param>
    /// <returns>La entidad encontrada o null si no existe.</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>Obtiene todas las entidades activas.</summary>
    /// <returns>Lista de entidades activas.</returns>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>Agrega una nueva entidad y retorna el Id asignado.</summary>
    /// <param name="entity">Entidad a crear.</param>
    /// <returns>El Id generado (IDENTITY).</returns>
    Task<int> AddAsync(T entity);

    /// <summary>Actualiza una entidad existente.</summary>
    /// <param name="entity">Entidad con los datos modificados.</param>
    Task UpdateAsync(T entity);

    /// <summary>Elimina (soft delete) una entidad.</summary>
    /// <param name="id">Id de la entidad a desactivar.</param>
    Task DeleteAsync(int id);

    /// <summary>Cuenta la cantidad de entidades activas.</summary>
    /// <returns>Número total de entidades activas.</returns>
    Task<int> CountAsync();
}
