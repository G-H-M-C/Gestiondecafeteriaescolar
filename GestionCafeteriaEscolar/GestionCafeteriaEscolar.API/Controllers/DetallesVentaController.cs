using GestionCafeteriaEscolar.API.Dtos;
using GestionCafeteriaEscolar.Domain.Entities;
using GestionCafeteriaEscolar.Infrastructure.Core;
using GestionCafeteriaEscolar.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetallesVentaController : ControllerBase
{
    private readonly DetalleVentaRepository _detalleRepository;
    private readonly GenericRepository<Venta> _ventaRepository;
    private readonly GenericRepository<Producto> _productoRepository;

    public DetallesVentaController(
        DetalleVentaRepository detalleRepository,
        GenericRepository<Venta> ventaRepository,
        GenericRepository<Producto> productoRepository)
    {
        _detalleRepository = detalleRepository;
        _ventaRepository = ventaRepository;
        _productoRepository = productoRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DetalleVentaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DetalleVentaResponseDto>>> GetAll()
    {
        var detalles = await _detalleRepository.GetAllAsync();

        return Ok(detalles.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DetalleVentaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetalleVentaResponseDto>> GetById(int id)
    {
        var detalle = await _detalleRepository.GetByIdAsync(id);
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
        var venta = await _ventaRepository.GetByIdAsync(dto.VentaId);
        if (venta is null)
        {
            return BadRequest($"La venta con Id {dto.VentaId} no existe.");
        }

        var producto = await _productoRepository.GetByIdAsync(dto.ProductoId);
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

        await _detalleRepository.AddAsync(detalle);
        await RecalcularTotalAsync(detalle.VentaId);

        return CreatedAtAction(nameof(GetById), new { id = detalle.Id }, ToResponse(detalle));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(DetalleVentaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetalleVentaResponseDto>> Update(int id, DetalleVentaUpdateDto dto)
    {
        var detalle = await _detalleRepository.GetByIdAsync(id);
        if (detalle is null)
        {
            return NotFound();
        }

        var venta = await _ventaRepository.GetByIdAsync(dto.VentaId);
        if (venta is null)
        {
            return BadRequest($"La venta con Id {dto.VentaId} no existe.");
        }

        var producto = await _productoRepository.GetByIdAsync(dto.ProductoId);
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
        await _detalleRepository.UpdateAsync(detalle);

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
        var detalle = await _detalleRepository.GetByIdAsync(id);
        if (detalle is null)
        {
            return NotFound();
        }

        var ventaId = detalle.VentaId;

        await _detalleRepository.DeleteAsync(detalle);
        await RecalcularTotalAsync(ventaId);

        return NoContent();
    }

    private async Task RecalcularTotalAsync(int ventaId)
    {
        var venta = await _ventaRepository.GetByIdAsync(ventaId);
        if (venta is null)
        {
            return;
        }

        venta.Total = await _detalleRepository.SumSubtotalByVentaAsync(ventaId);
        await _ventaRepository.UpdateAsync(venta);
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