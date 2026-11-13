using GestionCafeteriaEscolar.API.Dtos;
using GestionCafeteriaEscolar.Domain.Entities;
using GestionCafeteriaEscolar.Infrastructure.Core;
using GestionCafeteriaEscolar.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly GenericRepository<Producto> _productoRepository;
    private readonly DetalleVentaRepository _detalleVentaRepository;

    public ProductosController(
        GenericRepository<Producto> productoRepository,
        DetalleVentaRepository detalleVentaRepository)
    {
        _productoRepository = productoRepository;
        _detalleVentaRepository = detalleVentaRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetAll()
    {
        var productos = await _productoRepository.GetAllAsync();

        return Ok(productos.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoResponseDto>> GetById(int id)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(producto));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductoResponseDto>> Create(ProductoCreateDto dto)
    {
        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Precio = dto.Precio,
            Stock = dto.Stock
        };

        await _productoRepository.AddAsync(producto);

        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, ToResponse(producto));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoResponseDto>> Update(int id, ProductoUpdateDto dto)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto is null)
        {
            return NotFound();
        }

        producto.Nombre = dto.Nombre;
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;
        await _productoRepository.UpdateAsync(producto);

        return Ok(ToResponse(producto));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto is null)
        {
            return NotFound();
        }

        var tieneDetalles = await _detalleVentaRepository.AnyByProductoIdAsync(id);
        if (tieneDetalles)
        {
            return BadRequest("No se puede eliminar el producto porque tiene ventas registradas.");
        }

        await _productoRepository.DeleteAsync(producto);

        return NoContent();
    }

    private static ProductoResponseDto ToResponse(Producto producto)
    {
        return new ProductoResponseDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Stock = producto.Stock
        };
    }
}