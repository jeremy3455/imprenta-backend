using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImprentaSR.WebAPI.Controllers;

/// <summary>
/// Controlador REST para la gestión de clientes.
/// Expone endpoints CRUD sobre la ruta /api/clientes.
/// Todos los endpoints requieren autenticación JWT.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// <summary>Obtiene todos los clientes activos.</summary>
    /// <response code="200">Lista de clientes activos.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClienteDto>>> GetAll()
    {
        var clientes = await _clienteService.GetAllAsync();
        return Ok(clientes);
    }

    /// <summary>Obtiene un cliente por su Id.</summary>
    /// <param name="id">Id del cliente (int).</param>
    /// <response code="200">Cliente encontrado.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteDto>> GetById(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);
        if (cliente is null)
            return NotFound(new { message = $"Cliente con Id {id} no encontrado." });

        return Ok(cliente);
    }

    /// <summary>Crea un nuevo cliente.</summary>
    /// <response code="201">Cliente creado exitosamente.</response>
    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Create(CreateClienteDto dto)
    {
        var cliente = await _clienteService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    /// <summary>Actualiza los datos de un cliente existente.</summary>
    /// <response code="200">Cliente actualizado.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClienteDto>> Update(int id, UpdateClienteDto dto)
    {
        try
        {
            var cliente = await _clienteService.UpdateAsync(id, dto);
            return Ok(cliente);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Elimina (soft delete) un cliente.</summary>
    /// <response code="204">Cliente eliminado.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _clienteService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
