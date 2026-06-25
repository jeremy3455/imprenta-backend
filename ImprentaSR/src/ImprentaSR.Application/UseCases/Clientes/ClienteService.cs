using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;
using ImprentaSR.Domain.ValueObjects;

namespace ImprentaSR.Application.UseCases.Clientes;

/// <summary>
/// Servicio de aplicación que implementa los casos de uso para la gestión de clientes.
/// Orquesta las operaciones entre el dominio y la infraestructura.
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IRepository<Cliente> _clienteRepository;

    /// <summary>
    /// Constructor que inyecta el repositorio de clientes.
    /// </summary>
    /// <param name="clienteRepository">Repositorio genérico de clientes.</param>
    public ClienteService(IRepository<Cliente> clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    /// <summary>
    /// Obtiene todos los clientes activos del sistema.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de clientes activos mapeados a DTOs.</returns>
    public async Task<IReadOnlyList<ClienteDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.GetAllAsync(cancellationToken);
        return clientes.Select(MapToDto).ToList().AsReadOnly();
    }

    /// <summary>
    /// Obtiene un cliente por su identificador único.
    /// </summary>
    /// <param name="id">Id del cliente.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El cliente encontrado o null si no existe.</returns>
    public async Task<ClienteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        return cliente is not null ? MapToDto(cliente) : null;
    }

    /// <summary>
    /// Crea un nuevo cliente a partir de los datos del DTO de creación.
    /// Construye el Value Object Direccion y delega la persistencia al repositorio.
    /// </summary>
    /// <param name="dto">Datos del nuevo cliente.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El cliente creado con su Id asignado.</returns>
    public async Task<ClienteDto> CreateAsync(CreateClienteDto dto, CancellationToken cancellationToken = default)
    {
        var direccion = new Direccion(dto.Calle, dto.Ciudad, dto.Estado, dto.CodigoPostal);
        var cliente = new Cliente(dto.Nombre, dto.Email, dto.Telefono, direccion);

        var created = await _clienteRepository.AddAsync(cliente, cancellationToken);
        return MapToDto(created);
    }

    /// <summary>
    /// Actualiza los datos de un cliente existente.
    /// Lanza KeyNotFoundException si el cliente no existe.
    /// </summary>
    /// <param name="id">Id del cliente a actualizar.</param>
    /// <param name="dto">Nuevos datos del cliente.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El cliente actualizado.</returns>
    /// <exception cref="KeyNotFoundException">Si no se encuentra un cliente con el Id especificado.</exception>
    public async Task<ClienteDto> UpdateAsync(Guid id, UpdateClienteDto dto, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente with id {id} not found.");

        var direccion = new Direccion(dto.Calle, dto.Ciudad, dto.Estado, dto.CodigoPostal);
        cliente.UpdateData(dto.Nombre, dto.Email, dto.Telefono, direccion);

        await _clienteRepository.UpdateAsync(cliente, cancellationToken);
        return MapToDto(cliente);
    }

    /// <summary>
    /// Elimina (soft delete) un cliente por su Id.
    /// Lanza KeyNotFoundException si el cliente no existe.
    /// </summary>
    /// <param name="id">Id del cliente a eliminar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <exception cref="KeyNotFoundException">Si no se encuentra un cliente con el Id especificado.</exception>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente with id {id} not found.");

        await _clienteRepository.DeleteAsync(cliente, cancellationToken);
    }

    /// <summary>
    /// Mapea una entidad Cliente del dominio a un ClienteDto de la capa de aplicación.
    /// </summary>
    /// <param name="cliente">Entidad de dominio.</param>
    /// <returns>DTO con los datos del cliente.</returns>
    private static ClienteDto MapToDto(Cliente cliente) => new()
    {
        Id = cliente.Id,
        Nombre = cliente.Nombre,
        Email = cliente.Email,
        Telefono = cliente.Telefono,
        Direccion = cliente.Direccion.ToString(),
        Status = cliente.Status.ToString(),
        CreatedAt = cliente.CreatedAt
    };
}
