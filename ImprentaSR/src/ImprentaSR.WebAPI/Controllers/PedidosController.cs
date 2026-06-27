using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImprentaSR.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<PedidoResumenDto>>> GetAll(
        [FromQuery] PedidoFiltroDto filtro)
    {
        var result = await _pedidoService.GetAllAsync(filtro);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PedidoDetalleDto>> GetById(int id)
    {
        var pedido = await _pedidoService.GetByIdAsync(id);
        if (pedido is null)
            return NotFound(new { message = $"Pedido con Id {id} no encontrado." });
        return Ok(pedido);
    }

    [HttpPost]
    public async Task<ActionResult<PedidoDetalleDto>> Create(PedidoCreateDto dto)
    {
        try
        {
            var pedido = await _pedidoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PedidoDetalleDto>> Update(int id, PedidoCreateDto dto)
    {
        try
        {
            var pedido = await _pedidoService.UpdateAsync(id, dto);
            return Ok(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/aprobar")]
    public async Task<ActionResult<PedidoDetalleDto>> Aprobar(int id)
    {
        try
        {
            var pedido = await _pedidoService.AprobarAsync(id);
            return Ok(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/iniciar-produccion")]
    public async Task<ActionResult<PedidoDetalleDto>> IniciarProduccion(int id)
    {
        try
        {
            var pedido = await _pedidoService.IniciarProduccionAsync(id);
            return Ok(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/listo-entrega")]
    public async Task<ActionResult<PedidoDetalleDto>> MarcarListoEntrega(int id)
    {
        try
        {
            var pedido = await _pedidoService.MarcarListoEntregaAsync(id);
            return Ok(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/entregar")]
    public async Task<ActionResult<PedidoDetalleDto>> MarcarEntregado(int id)
    {
        try
        {
            var pedido = await _pedidoService.MarcarEntregadoAsync(id);
            return Ok(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/anular")]
    public async Task<ActionResult<PedidoDetalleDto>> Anular(int id, [FromBody] AnularDto dto)
    {
        try
        {
            var pedido = await _pedidoService.AnularAsync(id, dto.Motivo);
            return Ok(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException)
        {
            return Conflict(new { message = "El pedido no puede anularse porque ya está en producción." });
        }
    }

    [HttpPatch("{pedidoId:int}/detalle/{detalleId:int}/datos-sri")]
    public async Task<ActionResult<PedidoDetalleDto>> CompletarDatosSri(
        int pedidoId, int detalleId, [FromBody] DatosSriDto dto)
    {
        try
        {
            var pedido = await _pedidoService.CompletarDatosSriAsync(pedidoId, detalleId, dto);
            return Ok(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
