using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;
using ImprentaSR.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ImprentaSR.Infrastructure.Repositories;

/// <summary>
/// Implementación genérica del repositorio con Entity Framework Core.
/// Proporciona las operaciones CRUD básicas para cualquier entidad del dominio.
/// Aplica soft delete desactivando la entidad en lugar de eliminarla físicamente.
/// </summary>
/// <typeparam name="T">Tipo de entidad que debe heredar de <see cref="BaseEntity"/>.</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    /// <summary>Contexto de base de datos.</summary>
    protected readonly AppDbContext Context;

    /// <summary>DbSet genérico para la entidad T.</summary>
    protected readonly DbSet<T> DbSet;

    /// <summary>
    /// Constructor que recibe el contexto de base de datos.
    /// </summary>
    /// <param name="context">Contexto de EF Core.</param>
    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    /// <summary>
    /// Obtiene una entidad por su Id.
    /// </summary>
    /// <param name="id">Identificador único de la entidad.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>La entidad encontrada o null.</returns>
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las entidades activas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de entidades activas.</returns>
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(e => e.IsActive).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Agrega una nueva entidad y persiste los cambios.
    /// </summary>
    /// <param name="entity">Entidad a crear.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>La entidad creada.</returns>
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <summary>
    /// Actualiza una entidad existente y persiste los cambios.
    /// </summary>
    /// <param name="entity">Entidad con datos modificados.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina lógicamente (soft delete) una entidad desactivándola.
    /// </summary>
    /// <param name="entity">Entidad a desactivar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.Deactivate();
        DbSet.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Cuenta las entidades activas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Cantidad de entidades activas.</returns>
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(e => e.IsActive).CountAsync(cancellationToken);
    }
}
