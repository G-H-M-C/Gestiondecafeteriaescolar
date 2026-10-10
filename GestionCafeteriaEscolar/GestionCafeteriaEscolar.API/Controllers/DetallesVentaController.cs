using GestionCafeteriaEscolar.API.Data;
using GestionCafeteriaEscolar.API.Dtos;
using GestionCafeteriaEscolar.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetallesVentaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DetallesVentaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DetalleVentaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DetalleVentaResponseDto>>> GetAll()
    {
        var detalles = await _context.DetallesVenta
            .AsNoTracking()
            .OrderBy(d => d.Id)
            .ToListAsync();

        return Ok(detalles.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DetalleVentaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetalleVentaResponseDto>> GetById(int id)
    {
        var detalle = await _context.DetallesVenta.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        if (detalle is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(detalle));
    }

    [HttpPost]
    [ProducesResponseType(typeof(DetalleVentaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DetalleVentaResponseDto>> Create(DetalleVentaCreateDto dto)
    {
        var ventaExiste = await _context.Ventas.AnyAsync(v => v.Id == dto.VentaId);
        if (!ventaExiste)
        {
            return BadRequest($"La venta con Id {dto.VentaId} no existe.");
        }

        var producto = await _context.Productos.FindAsync(dto.ProductoId);
        if (producto is null)
        {
            return BadRequest($"El producto con Id {dto.ProductoId} no existe.");
        }

        var detalle = new DetalleVenta
        {
            VentaId = dto.VentaId,
            ProductoId = dto.ProductoId,
            Cantidad = dto.Cantidad,
            PrecioUnitario = producto.Precio,
            Subtotal = producto.Precio * dto.Cantidad
        };

        _context.DetallesVenta.Add(detalle);
        await _context.SaveChangesAsync();
        await RecalcularTotalAsync(detalle.VentaId);

        return CreatedAtAction(nameof(GetById), new { id = detalle.Id }, ToResponse(detalle));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(DetalleVentaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetalleVentaResponseDto>> Update(int id, DetalleVentaUpdateDto dto)
    {
        var detalle = await _context.DetallesVenta.FindAsync(id);
        if (detalle is null)
        {
            return NotFound();
        }

        var ventaExiste = await _context.Ventas.AnyAsync(v => v.Id == dto.VentaId);
        if (!ventaExiste)
        {
            return BadRequest($"La venta con Id {dto.VentaId} no existe.");
        }

        var producto = await _context.Productos.FindAsync(dto.ProductoId);
        if (producto is null)
        {
            return BadRequest($"El producto con Id {dto.ProductoId} no existe.");
        }

        var ventaAnteriorId = detalle.VentaId;

        detalle.VentaId = dto.VentaId;
        detalle.ProductoId = dto.ProductoId;
        detalle.Cantidad = dto.Cantidad;
        detalle.PrecioUnitario = producto.Precio;
        detalle.Subtotal = producto.Precio * dto.Cantidad;
        await _context.SaveChangesAsync();

        await RecalcularTotalAsync(detalle.VentaId);
        if (ventaAnteriorId != detalle.VentaId)
        {
            await RecalcularTotalAsync(ventaAnteriorId);
        }

        return Ok(ToResponse(detalle));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var detalle = await _context.DetallesVenta.FindAsync(id);
        if (detalle is null)
        {
            return NotFound();
        }

        var ventaId = detalle.VentaId;

        _context.DetallesVenta.Remove(detalle);
        await _context.SaveChangesAsync();
        await RecalcularTotalAsync(ventaId);

        return NoContent();
    }

    private async Task RecalcularTotalAsync(int ventaId)
    {
        var venta = await _context.Ventas.FindAsync(ventaId);
        if (venta is null)
        {
            return;
        }

        venta.Total = await _context.DetallesVenta
            .Where(d => d.VentaId == ventaId)
            .SumAsync(d => d.Subtotal);
        await _context.SaveChangesAsync();
    }

    private static DetalleVentaResponseDto ToResponse(DetalleVenta detalle)
    {
        return new DetalleVentaResponseDto
        {
            Id = detalle.Id,
            VentaId = detalle.VentaId,
            ProductoId = detalle.ProductoId,
            Cantidad = detalle.Cantidad,
            PrecioUnitario = detalle.PrecioUnitario,
            Subtotal = detalle.Subtotal
        };
    }
}