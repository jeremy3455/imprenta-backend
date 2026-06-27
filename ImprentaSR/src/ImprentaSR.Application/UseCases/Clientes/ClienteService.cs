using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using ImprentaSR.Domain.Entities;
using ImprentaSR.Domain.Interfaces;

namespace ImprentaSR.Application.UseCases.Clientes;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ISriValidator _sriValidator;

    public ClienteService(IClienteRepository clienteRepository, ISriValidator sriValidator)
    {
        _clienteRepository = clienteRepository;
        _sriValidator = sriValidator;
    }

    public async Task<IReadOnlyList<ClienteDto>> GetAllAsync(bool? estado = null)
    {
        var clientes = await _clienteRepository.GetAllAsync();
        if (estado.HasValue)
            clientes = clientes.Where(c => c.Estado == estado.Value).ToList().AsReadOnly();
        return clientes.Select(MapToDto).ToList().AsReadOnly();
    }

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        return cliente is not null ? MapToDto(cliente) : null;
    }

    public async Task<ClienteDto> CreateAsync(CreateClienteDto dto)
    {
        var error = _sriValidator.ValidarNumero(dto.NumeroCedulaRuc);
        if (error is not null)
            throw new ArgumentException(error);

        if (await _clienteRepository.ExistsByNumeroCedulaRucAsync(dto.NumeroCedulaRuc))
            throw new ArgumentException("Ya existe un cliente con ese número de cédula/RUC.");

        var cliente = new Cliente(
            dto.NumeroCedulaRuc, dto.RazonSocial, dto.Direccion,
            dto.Email, dto.Telefono, dto.TipoContribuyente);

        var id = await _clienteRepository.AddAsync(cliente);
        var created = await _clienteRepository.GetByIdAsync(id);
        return MapToDto(created!);
    }

    public async Task<ClienteDto> UpdateAsync(int id, UpdateClienteDto dto)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente con Id {id} no encontrado.");

        var error = _sriValidator.ValidarNumero(dto.NumeroCedulaRuc);
        if (error is not null)
            throw new ArgumentException(error);

        cliente.Update(dto.NumeroCedulaRuc, dto.RazonSocial, dto.Direccion,
            dto.Email, dto.Telefono, dto.TipoContribuyente);

        await _clienteRepository.UpdateAsync(cliente);
        return MapToDto(cliente);
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
            throw new KeyNotFoundException($"Cliente con Id {id} no encontrado.");

        await _clienteRepository.DeleteAsync(id);
    }

    private static ClienteDto MapToDto(Cliente cliente) => new()
    {
        Id = cliente.Id,
        NumeroCedulaRuc = cliente.NumeroCedulaRuc,
        RazonSocial = cliente.RazonSocial,
        Direccion = cliente.Direccion,
        Email = cliente.Email,
        Telefono = cliente.Telefono,
        TipoContribuyente = cliente.TipoContribuyente,
        Estado = cliente.Estado,
        FechaRegistro = cliente.FechaRegistro
    };
}
