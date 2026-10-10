using GestionCafeteriaEscolar.API.Dtos;
using GestionCafeteriaEscolar.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstudiantesController : ControllerBase
{
    // Lista en memoria (se reemplazará por base de datos en la Tarea 2)
    public static readonly List<Estudiante> Estudiantes = new()
    {
        new Estudiante { Id = 1, Nombre = "Ana Torres", Matricula = "EST-001", Grado = "5to A" },
        new Estudiante { Id = 2, Nombre = "Luis Rojas", Matricula = "EST-002", Grado = "6to B" }
    };

    private static int _siguienteId = 3;

    // GET api/estudiantes  -> 200
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<EstudianteResponseDto>> GetAll()
    {
        return Ok(Estudiantes.Select(ToResponse));
    }

    // GET api/estudiantes/1  -> 200 o 404
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EstudianteResponseDto> GetById(int id)
    {
        var estudiante = Estudiantes.FirstOrDefault(e => e.Id == id);
        if (estudiante is null)
            return NotFound(new { mensaje = $"No existe un estudiante con Id {id}." });

        return Ok(ToResponse(estudiante));
    }

    // POST api/estudiantes  -> 201 o 400
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<EstudianteResponseDto> Create(EstudianteCreateDto dto)
    {
        if (Estudiantes.Any(e => e.Matricula.Equals(dto.Matricula, StringComparison.OrdinalIgnoreCase)))
            return BadRequest(new { mensaje = $"Ya existe un estudiante con la matrícula {dto.Matricula}." });

        var estudiante = new Estudiante
        {
            Id = _siguienteId++,
            Nombre = dto.Nombre,
            Matricula = dto.Matricula,
            Grado = dto.Grado
        };

        Estudiantes.Add(estudiante);
        return CreatedAtAction(nameof(GetById), new { id = estudiante.Id }, ToResponse(estudiante));
    }

    // PUT api/estudiantes/1  -> 200, 400 o 404
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EstudianteResponseDto> Update(int id, EstudianteUpdateDto dto)
    {
        var estudiante = Estudiantes.FirstOrDefault(e => e.Id == id);
        if (estudiante is null)
            return NotFound(new { mensaje = $"No existe un estudiante con Id {id}." });

        if (Estudiantes.Any(e => e.Id != id && e.Matricula.Equals(dto.Matricula, StringComparison.OrdinalIgnoreCase)))
            return BadRequest(new { mensaje = $"Ya existe un estudiante con la matrícula {dto.Matricula}." });

        estudiante.Nombre = dto.Nombre;
        estudiante.Matricula = dto.Matricula;
        estudiante.Grado = dto.Grado;

        return Ok(ToResponse(estudiante));
    }

    // DELETE api/estudiantes/1  -> 204 o 404
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var estudiante = Estudiantes.FirstOrDefault(e => e.Id == id);
        if (estudiante is null)
            return NotFound(new { mensaje = $"No existe un estudiante con Id {id}." });

        Estudiantes.Remove(estudiante);
        return NoContent();
    }

    private static EstudianteResponseDto ToResponse(Estudiante e) => new()
    {
        Id = e.Id,
        Nombre = e.Nombre,
        Matricula = e.Matricula,
        Grado = e.Grado
    };
}