using ImprentaSR.Application.DTOs;

namespace ImprentaSR.Application.Interfaces;

public interface IClienteService
{
    Task<IReadOnlyList<ClienteDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClienteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClienteDto> CreateAsync(CreateClienteDto dto, CancellationToken cancellationToken = default);
    Task<ClienteDto> UpdateAsync(Guid id, UpdateClienteDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
