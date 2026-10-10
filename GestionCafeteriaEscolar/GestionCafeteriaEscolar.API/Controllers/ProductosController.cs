using GestionCafeteriaEscolar.API.Dtos;
using GestionCafeteriaEscolar.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    // Lista en memoria (se reemplazará por base de datos en la Tarea 2)
    public static readonly List<Producto> Productos = new()
    {
        new Producto { Id = 1, Nombre = "Sándwich de jamón", Precio = 25m, Stock = 50 },
        new Producto { Id = 2, Nombre = "Jugo de naranja", Precio = 15m, Stock = 40 }
    };

    private static int _siguienteId = 3;

    // GET api/productos  -> 200
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ProductoResponseDto>> GetAll()
    {
        return Ok(Productos.Select(ToResponse));
    }

    // GET api/productos/1  -> 200 o 404
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProductoResponseDto> GetById(int id)
    {
        var producto = Productos.FirstOrDefault(p => p.Id == id);
        if (producto is null)
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });

        return Ok(ToResponse(producto));
    }

    // POST api/productos  -> 201 o 400
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ProductoResponseDto> Create(ProductoCreateDto dto)
    {
        var producto = new Producto
        {
            Id = _siguienteId++,
            Nombre = dto.Nombre,
            Precio = dto.Precio,
            Stock = dto.Stock
        };

        Productos.Add(producto);
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, ToResponse(producto));
    }

    // PUT api/productos/1  -> 200, 400 o 404
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProductoResponseDto> Update(int id, ProductoUpdateDto dto)
    {
        var producto = Productos.FirstOrDefault(p => p.Id == id);
        if (producto is null)
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });

        producto.Nombre = dto.Nombre;
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;

        return Ok(ToResponse(producto));
    }

    // DELETE api/productos/1  -> 204 o 404
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var producto = Productos.FirstOrDefault(p => p.Id == id);
        if (producto is null)
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });

        Productos.Remove(producto);
        return NoContent();
    }

    private static ProductoResponseDto ToResponse(Producto p) => new()
    {
        Id = p.Id,
        Nombre = p.Nombre,
        Precio = p.Precio,
        Stock = p.Stock
    };
}