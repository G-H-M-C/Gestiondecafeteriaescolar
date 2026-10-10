using GestionCafeteriaEscolar.API.Dtos;
using GestionCafeteriaEscolar.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetallesVentaController : ControllerBase
{
    // Lista en memoria (se reemplazará por base de datos en la Tarea 2)
    public static readonly List<DetalleVenta> Detalles = new();

    private static int _siguienteId = 1;

    // GET api/detallesventa  -> 200
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<DetalleVentaResponseDto>> GetAll()
    {
        return Ok(Detalles.Select(ToResponse));
    }

    // GET api/detallesventa/1  -> 200 o 404
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DetalleVentaResponseDto> GetById(int id)
    {
        var detalle = Detalles.FirstOrDefault(d => d.Id == id);
        if (detalle is null)
            return NotFound(new { mensaje = $"No existe un detalle de venta con Id {id}." });

        return Ok(ToResponse(detalle));
    }

    // POST api/detallesventa  -> 201 o 400
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<DetalleVentaResponseDto> Create(DetalleVentaCreateDto dto)
    {
        if (!VentasController.Ventas.Any(v => v.Id == dto.VentaId))
            return BadRequest(new { mensaje = $"La venta con Id {dto.VentaId} no existe." });

        var producto = ProductosController.Productos.FirstOrDefault(p => p.Id == dto.ProductoId);
        if (producto is null)
            return BadRequest(new { mensaje = $"El producto con Id {dto.ProductoId} no existe." });

        var detalle = new DetalleVenta
        {
            Id = _siguienteId++,
            VentaId = dto.VentaId,
            ProductoId = dto.ProductoId,
            Cantidad = dto.Cantidad,
            PrecioUnitario = producto.Precio,              // el precio sale del producto
            Subtotal = producto.Precio * dto.Cantidad      // el subtotal se calcula
        };

        Detalles.Add(detalle);
        RecalcularTotal(detalle.VentaId);

        return CreatedAtAction(nameof(GetById), new { id = detalle.Id }, ToResponse(detalle));
    }

    // PUT api/detallesventa/1  -> 200, 400 o 404
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DetalleVentaResponseDto> Update(int id, DetalleVentaUpdateDto dto)
    {
        var detalle = Detalles.FirstOrDefault(d => d.Id == id);
        if (detalle is null)
            return NotFound(new { mensaje = $"No existe un detalle de venta con Id {id}." });

        if (!VentasController.Ventas.Any(v => v.Id == dto.VentaId))
            return BadRequest(new { mensaje = $"La venta con Id {dto.VentaId} no existe." });

        var producto = ProductosController.Productos.FirstOrDefault(p => p.Id == dto.ProductoId);
        if (producto is null)
            return BadRequest(new { mensaje = $"El producto con Id {dto.ProductoId} no existe." });

        var ventaAnterior = detalle.VentaId;

        detalle.VentaId = dto.VentaId;
        detalle.ProductoId = dto.ProductoId;
        detalle.Cantidad = dto.Cantidad;
        detalle.PrecioUnitario = producto.Precio;
        detalle.Subtotal = producto.Precio * dto.Cantidad;

        RecalcularTotal(ventaAnterior);
        RecalcularTotal(detalle.VentaId);

        return Ok(ToResponse(detalle));
    }

    // DELETE api/detallesventa/1  -> 204 o 404
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var detalle = Detalles.FirstOrDefault(d => d.Id == id);
        if (detalle is null)
            return NotFound(new { mensaje = $"No existe un detalle de venta con Id {id}." });

        Detalles.Remove(detalle);
        RecalcularTotal(detalle.VentaId);
        return NoContent();
    }

    // Recalcula el total de una venta sumando los subtotales de sus detalles
    private static void RecalcularTotal(int ventaId)
    {
        var venta = VentasController.Ventas.FirstOrDefault(v => v.Id == ventaId);
        if (venta is null) return;

        venta.Total = Detalles.Where(d => d.VentaId == ventaId).Sum(d => d.Subtotal);
    }

    private static DetalleVentaResponseDto ToResponse(DetalleVenta d) => new()
    {
        Id = d.Id,
        VentaId = d.VentaId,
        ProductoId = d.ProductoId,
        Cantidad = d.Cantidad,
        PrecioUnitario = d.PrecioUnitario,
        Subtotal = d.Subtotal
    };
}