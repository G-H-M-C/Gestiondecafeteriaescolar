using GestionCafeteriaEscolar.API.Dtos;
using GestionCafeteriaEscolar.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    // Lista en memoria (se reemplazará por base de datos en la Tarea 2)
    public static readonly List<Venta> Ventas = new();

    private static int _siguienteId = 1;

    // GET api/ventas  -> 200
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<VentaResponseDto>> GetAll()
    {
        return Ok(Ventas.Select(ToResponse));
    }

    // GET api/ventas/1  -> 200 o 404
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VentaResponseDto> GetById(int id)
    {
        var venta = Ventas.FirstOrDefault(v => v.Id == id);
        if (venta is null)
            return NotFound(new { mensaje = $"No existe una venta con Id {id}." });

        return Ok(ToResponse(venta));
    }

    // POST api/ventas  -> 201 o 400
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<VentaResponseDto> Create(VentaCreateDto dto)
    {
        if (!EstudiantesController.Estudiantes.Any(e => e.Id == dto.EstudianteId))
            return BadRequest(new { mensaje = $"El estudiante con Id {dto.EstudianteId} no existe." });

        var venta = new Venta
        {
            Id = _siguienteId++,
            EstudianteId = dto.EstudianteId,
            Fecha = DateTime.Now,   // la fecha la asigna el servidor
            Total = 0               // el total se calcula con los detalles
        };

        Ventas.Add(venta);
        return CreatedAtAction(nameof(GetById), new { id = venta.Id }, ToResponse(venta));
    }

    // PUT api/ventas/1  -> 200, 400 o 404
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VentaResponseDto> Update(int id, VentaUpdateDto dto)
    {
        var venta = Ventas.FirstOrDefault(v => v.Id == id);
        if (venta is null)
            return NotFound(new { mensaje = $"No existe una venta con Id {id}." });

        if (!EstudiantesController.Estudiantes.Any(e => e.Id == dto.EstudianteId))
            return BadRequest(new { mensaje = $"El estudiante con Id {dto.EstudianteId} no existe." });

        venta.EstudianteId = dto.EstudianteId;
        return Ok(ToResponse(venta));
    }

    // DELETE api/ventas/1  -> 204 o 404
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var venta = Ventas.FirstOrDefault(v => v.Id == id);
        if (venta is null)
            return NotFound(new { mensaje = $"No existe una venta con Id {id}." });

        Ventas.Remove(venta);
        DetallesVentaController.Detalles.RemoveAll(d => d.VentaId == id);
        return NoContent();
    }

    private static VentaResponseDto ToResponse(Venta v) => new()
    {
        Id = v.Id,
        EstudianteId = v.EstudianteId,
        Fecha = v.Fecha,
        Total = v.Total
    };
}
