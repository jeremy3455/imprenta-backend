using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using ImprentaSR.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImprentaSR.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly SriService _sriService;
    private readonly ISriValidator _sriValidator;

    public ClientesController(IClienteService clienteService, SriService sriService, ISriValidator sriValidator)
    {
        _clienteService = clienteService;
        _sriService = sriService;
        _sriValidator = sriValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClienteDto>>> GetAll([FromQuery] bool? estado = null)
    {
        var clientes = await _clienteService.GetAllAsync(estado);
        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteDto>> GetById(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);
        if (cliente is null)
            return NotFound(new { message = $"Cliente con Id {id} no encontrado." });
        return Ok(cliente);
    }

    [HttpGet("sri/{numero}")]
    public async Task<ActionResult<SriRespuestaDto>> ConsultarSri(string numero)
    {
        var error = _sriValidator.ValidarNumero(numero);
        if (error is not null)
            return BadRequest(new { message = error });

        try
        {
            var resultado = await _sriService.ConsultarContribuyenteAsync(numero);
            if (!resultado.Encontrado)
                return NotFound(new { message = "Contribuyente no encontrado en el SRI." });
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(503, new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Create(CreateClienteDto dto)
    {
        try
        {
            var cliente = await _clienteService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

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
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

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
