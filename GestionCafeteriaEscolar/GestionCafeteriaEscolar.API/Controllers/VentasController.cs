using GestionCafeteriaEscolar.API.Data;
using GestionCafeteriaEscolar.API.Dtos;
using GestionCafeteriaEscolar.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public VentasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VentaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VentaResponseDto>>> GetAll()
    {
        var ventas = await _context.Ventas
            .AsNoTracking()
            .OrderBy(v => v.Id)
            .ToListAsync();

        return Ok(ventas.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VentaResponseDto>> GetById(int id)
    {
        var venta = await _context.Ventas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
        if (venta is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(venta));
    }

    [HttpPost]
    [ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VentaResponseDto>> Create(VentaCreateDto dto)
    {
        var estudianteExiste = await _context.Estudiantes.AnyAsync(e => e.Id == dto.EstudianteId);
        if (!estudianteExiste)
        {
            return BadRequest($"El estudiante con Id {dto.EstudianteId} no existe.");
        }

        var venta = new Venta
        {
            EstudianteId = dto.EstudianteId,
            Fecha = DateTime.Now,
            Total = 0
        };

        _context.Ventas.Add(venta);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = venta.Id }, ToResponse(venta));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VentaResponseDto>> Update(int id, VentaUpdateDto dto)
    {
        var venta = await _context.Ventas.FindAsync(id);
        if (venta is null)
        {
            return NotFound();
        }

        var estudianteExiste = await _context.Estudiantes.AnyAsync(e => e.Id == dto.EstudianteId);
        if (!estudianteExiste)
        {
            return BadRequest($"El estudiante con Id {dto.EstudianteId} no existe.");
        }

        venta.EstudianteId = dto.EstudianteId;
        await _context.SaveChangesAsync();

        return Ok(ToResponse(venta));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var venta = await _context.Ventas.FindAsync(id);
        if (venta is null)
        {
            return NotFound();
        }

        // Los detalles de la venta se eliminan en cascada en la base de datos.
        _context.Ventas.Remove(venta);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static VentaResponseDto ToResponse(Venta venta)
    {
        return new VentaResponseDto
        {
            Id = venta.Id,
            EstudianteId = venta.EstudianteId,
            Fecha = venta.Fecha,
            Total = venta.Total
        };
    }
}