using ImprentaSR.Application.DTOs;

namespace ImprentaSR.Application.Interfaces;

/// <summary>
/// Interfaz del servicio de clientes que define las operaciones de negocio disponibles.
/// Actúa como puerto de entrada para la capa de aplicación (Clean Architecture).
/// </summary>
public interface IClienteService
{
    /// <summary>Obtiene todos los clientes activos.</summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de clientes activos.</returns>
    Task<IReadOnlyList<ClienteDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Obtiene un cliente por su Id.</summary>
    /// <param name="id">Identificador del cliente.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El cliente encontrado o null si no existe.</returns>
    Task<ClienteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Crea un nuevo cliente.</summary>
    /// <param name="dto">Datos del cliente a crear.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El cliente creado con su Id asignado.</returns>
    Task<ClienteDto> CreateAsync(CreateClienteDto dto, CancellationToken cancellationToken = default);

    /// <summary>Actualiza los datos de un cliente existente.</summary>
    /// <param name="id">Id del cliente a actualizar.</param>
    /// <param name="dto">Nuevos datos del cliente.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El cliente actualizado.</returns>
    Task<ClienteDto> UpdateAsync(Guid id, UpdateClienteDto dto, CancellationToken cancellationToken = default);

    /// <summary>Elimina (soft delete) un cliente por su Id.</summary>
    /// <param name="id">Id del cliente a eliminar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
