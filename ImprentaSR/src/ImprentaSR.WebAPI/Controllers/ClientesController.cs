using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ImprentaSR.WebAPI.Controllers;

/// <summary>
/// Controlador REST para la gestión de clientes.
/// Expone endpoints CRUD sobre la ruta /api/clientes.
/// Delega toda la lógica de negocio al servicio <see cref="IClienteService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    /// <summary>
    /// Constructor que inyecta el servicio de clientes.
    /// </summary>
    /// <param name="clienteService">Servicio de aplicación de clientes.</param>
    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// <summary>
    /// Obtiene la lista completa de clientes activos.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de clientes activos.</returns>
    /// <response code="200">Lista de clientes obtenida correctamente.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClienteDto>>> GetAll(CancellationToken cancellationToken)
    {
        var clientes = await _clienteService.GetAllAsync(cancellationToken);
        return Ok(clientes);
    }

    /// <summary>
    /// Obtiene un cliente por su Id.
    /// </summary>
    /// <param name="id">Identificador único del cliente (GUID).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Datos del cliente solicitado.</returns>
    /// <response code="200">Cliente encontrado.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClienteDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
            return NotFound(new { message = $"Cliente with id {id} not found." });

        return Ok(cliente);
    }

    /// <summary>
    /// Crea un nuevo cliente.
    /// </summary>
    /// <param name="dto">Datos del cliente a crear.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El cliente creado con su Id asignado.</returns>
    /// <response code="201">Cliente creado exitosamente.</response>
    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Create(CreateClienteDto dto, CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    /// <summary>
    /// Actualiza los datos de un cliente existente.
    /// </summary>
    /// <param name="id">Id del cliente a actualizar.</param>
    /// <param name="dto">Nuevos datos del cliente.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El cliente actualizado.</returns>
    /// <response code="200">Cliente actualizado correctamente.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClienteDto>> Update(Guid id, UpdateClienteDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await _clienteService.UpdateAsync(id, dto, cancellationToken);
            return Ok(cliente);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina (soft delete) un cliente por su Id.
    /// </summary>
    /// <param name="id">Id del cliente a eliminar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>NoContent si se eliminó correctamente.</returns>
    /// <response code="204">Cliente eliminado correctamente.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _clienteService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
