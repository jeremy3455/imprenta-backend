using System.Security.Claims;
using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using ImprentaSR.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImprentaSR.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolicitudesController : ControllerBase
{
    private readonly ISolicitudService _solicitudService;
    private readonly INotificacionService _notificacionService;
    private readonly IClienteService _clienteService;
    private readonly UsuarioRepository _usuarioRepository;

    public SolicitudesController(
        ISolicitudService solicitudService,
        INotificacionService notificacionService,
        IClienteService clienteService,
        UsuarioRepository usuarioRepository)
    {
        _solicitudService = solicitudService;
        _notificacionService = notificacionService;
        _clienteService = clienteService;
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<SolicitudResumenDto>>> GetAll(
        [FromQuery] string? estado,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var rol = User.FindFirstValue(ClaimTypes.Role);
        int? clienteId = null;

        if (rol == "Cliente")
        {
            var clienteIdClaim = User.FindFirstValue("ClienteId");
            if (!string.IsNullOrEmpty(clienteIdClaim))
                clienteId = int.Parse(clienteIdClaim);
        }

        var result = await _solicitudService.GetAllAsync(clienteId, estado, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SolicitudDetalleDto>> GetById(int id)
    {
        var solicitud = await _solicitudService.GetByIdAsync(id);
        if (solicitud is null)
            return NotFound(new { message = $"Solicitud con Id {id} no encontrada." });
        return Ok(solicitud);
    }

    [HttpPost]
    public async Task<ActionResult<SolicitudDetalleDto>> Create(SolicitudCreateDto dto)
    {
        var clienteIdClaim = User.FindFirstValue("ClienteId");
        if (string.IsNullOrEmpty(clienteIdClaim))
            return BadRequest(new { message = "El usuario no tiene un cliente asociado." });

        var clienteId = int.Parse(clienteIdClaim);

        try
        {
            var solicitud = await _solicitudService.CreateAsync(clienteId, dto);

            // Notificar a administradores
            var cliente = await _clienteService.GetByIdAsync(clienteId);
            var nombreCliente = cliente?.RazonSocial ?? "Cliente";
            var mensaje = $"El cliente {nombreCliente} ha realizado una nueva solicitud de productos.";
            var admins = await _usuarioRepository.GetAllAdminsAsync();
            foreach (var admin in admins)
            {
                await _notificacionService.CreateAsync(admin.Id, mensaje, "NuevaSolicitud", solicitud.Id);
            }

            return CreatedAtAction(nameof(GetById), new { id = solicitud.Id }, solicitud);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/aprobar")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<SolicitudDetalleDto>> Aprobar(int id)
    {
        try
        {
            var solicitud = await _solicitudService.AprobarAsync(id);
            return Ok(solicitud);
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

    [HttpPatch("{id:int}/rechazar")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<SolicitudDetalleDto>> Rechazar(int id)
    {
        try
        {
            var solicitud = await _solicitudService.RechazarAsync(id);
            return Ok(solicitud);
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
}
