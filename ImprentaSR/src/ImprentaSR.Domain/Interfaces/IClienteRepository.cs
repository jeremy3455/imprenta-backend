using ImprentaSR.Domain.Entities;

namespace ImprentaSR.Domain.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<bool> ExistsByNumeroCedulaRucAsync(string numero);
}
