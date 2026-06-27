using ImprentaSR.Application.DTOs;

namespace ImprentaSR.Application.Interfaces;

public interface IClienteService
{
    Task<IReadOnlyList<ClienteDto>> GetAllAsync(bool? estado = null);
    Task<ClienteDto?> GetByIdAsync(int id);
    Task<ClienteDto> CreateAsync(CreateClienteDto dto);
    Task<ClienteDto> UpdateAsync(int id, UpdateClienteDto dto);
    Task DeleteAsync(int id);
}
