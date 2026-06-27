using System.Security.Claims;
using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImprentaSR.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Operador")]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionService _notificacionService;

    public NotificacionesController(INotificacionService notificacionService)
    {
        _notificacionService = notificacionService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificacionDto>>> GetAll()
    {
        var usuarioId = GetUsuarioId();
        var items = await _notificacionService.GetByUsuarioAsync(usuarioId);
        return Ok(items);
    }

    [HttpGet("no-leidas")]
    public async Task<ActionResult<int>> CountNoLeidas()
    {
        var usuarioId = GetUsuarioId();
        var count = await _notificacionService.CountNoLeidasAsync(usuarioId);
        return Ok(count);
    }

    [HttpPatch("{id:int}/leer")]
    public async Task<ActionResult> MarcarComoLeida(int id)
    {
        await _notificacionService.MarcarComoLeidaAsync(id);
        return NoContent();
    }

    [HttpPatch("leer-todas")]
    public async Task<ActionResult> MarcarTodasComoLeidas()
    {
        var usuarioId = GetUsuarioId();
        await _notificacionService.MarcarTodasComoLeidasAsync(usuarioId);
        return NoContent();
    }

    private int GetUsuarioId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(claim!);
    }
}
