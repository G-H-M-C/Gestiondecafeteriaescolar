using GestionCafeteriaEscolar.API.Dtos;
using GestionCafeteriaEscolar.Domain.Entities;
using GestionCafeteriaEscolar.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstudiantesController : ControllerBase
{
    private readonly EstudianteRepository _estudianteRepository;

    public EstudiantesController(EstudianteRepository estudianteRepository)
    {
        _estudianteRepository = estudianteRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EstudianteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EstudianteResponseDto>>> GetAll()
    {
        var estudiantes = await _estudianteRepository.GetAllAsync();

        return Ok(estudiantes.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EstudianteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstudianteResponseDto>> GetById(int id)
    {
        var estudiante = await _estudianteRepository.GetByIdAsync(id);
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
        var matriculaExiste = await _estudianteRepository.ExistsMatriculaAsync(dto.Matricula);
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

        await _estudianteRepository.AddAsync(estudiante);

        return CreatedAtAction(nameof(GetById), new { id = estudiante.Id }, ToResponse(estudiante));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EstudianteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstudianteResponseDto>> Update(int id, EstudianteUpdateDto dto)
    {
        var estudiante = await _estudianteRepository.GetByIdAsync(id);
        if (estudiante is null)
        {
            return NotFound();
        }

        var matriculaExiste = await _estudianteRepository.ExistsMatriculaAsync(dto.Matricula, id);
        if (matriculaExiste)
        {
            return BadRequest($"Ya existe un estudiante con la matrícula {dto.Matricula}.");
        }

        estudiante.Nombre = dto.Nombre;
        estudiante.Matricula = dto.Matricula;
        estudiante.Grado = dto.Grado;
        await _estudianteRepository.UpdateAsync(estudiante);

        return Ok(ToResponse(estudiante));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var estudiante = await _estudianteRepository.GetByIdAsync(id);
        if (estudiante is null)
        {
            return NotFound();
        }

        var tieneVentas = await _estudianteRepository.HasVentasAsync(id);
        if (tieneVentas)
        {
            return BadRequest("No se puede eliminar el estudiante porque tiene ventas registradas.");
        }

        await _estudianteRepository.DeleteAsync(estudiante);

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