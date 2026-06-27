using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;

namespace ImprentaSR.Application.UseCases.Clientes;

/// <summary>
/// Servicio de aplicación que implementa los casos de uso para la gestión de clientes.
/// Orquesta las operaciones entre el dominio y el repositorio Dapper.
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IRepository<Cliente> _clienteRepository;

    /// <summary>
    /// Constructor que inyecta el repositorio de clientes.
    /// </summary>
    public ClienteService(IRepository<Cliente> clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    /// <summary>
    /// Obtiene todos los clientes activos.
    /// </summary>
    public async Task<IReadOnlyList<ClienteDto>> GetAllAsync()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        return clientes.Select(MapToDto).ToList().AsReadOnly();
    }

    /// <summary>
    /// Obtiene un cliente por su Id.
    /// </summary>
    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        return cliente is not null ? MapToDto(cliente) : null;
    }

    /// <summary>
    /// Crea un nuevo cliente a partir del DTO de creación.
    /// </summary>
    public async Task<ClienteDto> CreateAsync(CreateClienteDto dto)
    {
        var cliente = new Cliente(dto.Ruc, dto.RazonSocial, dto.Direccion, dto.Telefono, dto.Email);
        var id = await _clienteRepository.AddAsync(cliente);

        // Recuperar el cliente creado con el Id asignado
        var created = await _clienteRepository.GetByIdAsync(id);
        return MapToDto(created!);
    }

    /// <summary>
    /// Actualiza los datos de un cliente existente.
    /// Lanza KeyNotFoundException si el cliente no existe.
    /// </summary>
    public async Task<ClienteDto> UpdateAsync(int id, UpdateClienteDto dto)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente con Id {id} no encontrado.");

        cliente.Update(dto.Ruc, dto.RazonSocial, dto.Direccion, dto.Telefono, dto.Email);
        await _clienteRepository.UpdateAsync(cliente);

        return MapToDto(cliente);
    }

    /// <summary>
    /// Elimina (soft delete) un cliente por su Id.
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente con Id {id} no encontrado.");

        await _clienteRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Mapea una entidad Cliente a un ClienteDto.
    /// </summary>
    private static ClienteDto MapToDto(Cliente cliente) => new()
    {
        Id = cliente.Id,
        Ruc = cliente.Ruc,
        RazonSocial = cliente.RazonSocial,
        Direccion = cliente.Direccion,
        Telefono = cliente.Telefono,
        Email = cliente.Email,
        Activo = cliente.Activo
    };
}
