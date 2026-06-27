using ImprentaSR.Application.DTOs;

namespace ImprentaSR.Application.Interfaces;

/// <summary>
/// Interfaz del servicio de clientes.
/// Define las operaciones de negocio disponibles en la capa de aplicación.
/// </summary>
public interface IClienteService
{
    /// <summary>Obtiene todos los clientes activos.</summary>
    Task<IReadOnlyList<ClienteDto>> GetAllAsync();

    /// <summary>Obtiene un cliente por su Id.</summary>
    Task<ClienteDto?> GetByIdAsync(int id);

    /// <summary>Crea un nuevo cliente.</summary>
    Task<ClienteDto> CreateAsync(CreateClienteDto dto);

    /// <summary>Actualiza los datos de un cliente existente.</summary>
    Task<ClienteDto> UpdateAsync(int id, UpdateClienteDto dto);

    /// <summary>Elimina (soft delete) un cliente por su Id.</summary>
    Task DeleteAsync(int id);
}
