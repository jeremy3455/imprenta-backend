using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;
using ImprentaSR.Domain.ValueObjects;

namespace ImprentaSR.Application.UseCases.Clientes;

public class ClienteService : IClienteService
{
    private readonly IRepository<Cliente> _clienteRepository;

    public ClienteService(IRepository<Cliente> clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IReadOnlyList<ClienteDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.GetAllAsync(cancellationToken);
        return clientes.Select(MapToDto).ToList().AsReadOnly();
    }

    public async Task<ClienteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        return cliente is not null ? MapToDto(cliente) : null;
    }

    public async Task<ClienteDto> CreateAsync(CreateClienteDto dto, CancellationToken cancellationToken = default)
    {
        var direccion = new Direccion(dto.Calle, dto.Ciudad, dto.Estado, dto.CodigoPostal);
        var cliente = new Cliente(dto.Nombre, dto.Email, dto.Telefono, direccion);

        var created = await _clienteRepository.AddAsync(cliente, cancellationToken);
        return MapToDto(created);
    }

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

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente with id {id} not found.");

        await _clienteRepository.DeleteAsync(cliente, cancellationToken);
    }

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
