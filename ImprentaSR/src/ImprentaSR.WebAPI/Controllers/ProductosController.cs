using ImprentaSR.Application.DTOs;
using ImprentaSR.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImprentaSR.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductoDto>>> GetAll(
        [FromQuery] int? categoriaId,
        [FromQuery] bool? esDocumentoTributario,
        [FromQuery] bool? estado,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var filtro = new ProductoFiltroDto
        {
            CategoriaId = categoriaId,
            EsDocumentoTributario = esDocumentoTributario,
            Estado = estado,
            Search = search,
            Page = page,
            PageSize = pageSize,
        };
        var result = await _productoService.GetAllAsync(filtro);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductoDto>> GetById(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto is null)
            return NotFound(new { message = $"Producto con Id {id} no encontrado." });
        return Ok(producto);
    }

    [HttpGet("por-tipo-contribuyente/{tipo}")]
    public async Task<ActionResult<IReadOnlyList<ProductoDto>>> GetPorTipoContribuyente(string tipo)
    {
        var items = await _productoService.GetPorTipoContribuyenteAsync(tipo.ToUpper());
        return Ok(items);
    }

    [HttpGet("categorias")]
    public async Task<ActionResult<IReadOnlyList<CategoriaDto>>> GetCategorias()
    {
        var items = await _productoService.GetCategoriasAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ProductoDto>> Create(ProductoCreateDto dto)
    {
        try
        {
            var producto = await _productoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductoDto>> Update(int id, ProductoCreateDto dto)
    {
        try
        {
            var producto = await _productoService.UpdateAsync(id, dto);
            return Ok(producto);
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
            await _productoService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
