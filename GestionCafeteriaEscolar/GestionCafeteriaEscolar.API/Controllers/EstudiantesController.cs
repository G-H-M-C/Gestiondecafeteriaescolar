using GestionCafeteriaEscolar.API.Data;
using GestionCafeteriaEscolar.API.Dtos;
using GestionCafeteriaEscolar.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstudiantesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EstudiantesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EstudianteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EstudianteResponseDto>>> GetAll()
    {
        var estudiantes = await _context.Estudiantes
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToListAsync();

        return Ok(estudiantes.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EstudianteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstudianteResponseDto>> GetById(int id)
    {
        var estudiante = await _context.Estudiantes.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        if (estudiante is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(estudiante));
    }

    [HttpPost]
    [ProducesResponseType(typeof(EstudianteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EstudianteResponseDto>> Create(EstudianteCreateDto dto)
    {
        var matriculaExiste = await _context.Estudiantes.AnyAsync(e => e.Matricula == dto.Matricula);
        if (matriculaExiste)
        {
            return BadRequest($"Ya existe un estudiante con la matrícula {dto.Matricula}.");
        }

        var estudiante = new Estudiante
        {
            Nombre = dto.Nombre,
            Matricula = dto.Matricula,
            Grado = dto.Grado
        };

        _context.Estudiantes.Add(estudiante);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = estudiante.Id }, ToResponse(estudiante));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EstudianteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstudianteResponseDto>> Update(int id, EstudianteUpdateDto dto)
    {
        var estudiante = await _context.Estudiantes.FindAsync(id);
        if (estudiante is null)
        {
            return NotFound();
        }

        var matriculaExiste = await _context.Estudiantes
            .AnyAsync(e => e.Matricula == dto.Matricula && e.Id != id);
        if (matriculaExiste)
        {
            return BadRequest($"Ya existe un estudiante con la matrícula {dto.Matricula}.");
        }

        estudiante.Nombre = dto.Nombre;
        estudiante.Matricula = dto.Matricula;
        estudiante.Grado = dto.Grado;
        await _context.SaveChangesAsync();

        return Ok(ToResponse(estudiante));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var estudiante = await _context.Estudiantes.FindAsync(id);
        if (estudiante is null)
        {
            return NotFound();
        }

        var tieneVentas = await _context.Ventas.AnyAsync(v => v.EstudianteId == id);
        if (tieneVentas)
        {
            return BadRequest("No se puede eliminar el estudiante porque tiene ventas registradas.");
        }

        _context.Estudiantes.Remove(estudiante);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static EstudianteResponseDto ToResponse(Estudiante estudiante)
    {
        return new EstudianteResponseDto
        {
            Id = estudiante.Id,
            Nombre = estudiante.Nombre,
            Matricula = estudiante.Matricula,
            Grado = estudiante.Grado
        };
    }
}