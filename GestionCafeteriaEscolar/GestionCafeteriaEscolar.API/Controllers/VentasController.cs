using GestionCafeteriaEscolar.API.Dtos;
using GestionCafeteriaEscolar.Domain.Entities;
using GestionCafeteriaEscolar.Infrastructure.Core;
using GestionCafeteriaEscolar.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly GenericRepository<Venta> _ventaRepository;
    private readonly EstudianteRepository _estudianteRepository;

    public VentasController(
        GenericRepository<Venta> ventaRepository,
        EstudianteRepository estudianteRepository)
    {
        _ventaRepository = ventaRepository;
        _estudianteRepository = estudianteRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VentaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VentaResponseDto>>> GetAll()
    {
        var ventas = await _ventaRepository.GetAllAsync();

        return Ok(ventas.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VentaResponseDto>> GetById(int id)
    {
        var venta = await _ventaRepository.GetByIdAsync(id);
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
        var estudiante = await _estudianteRepository.GetByIdAsync(dto.EstudianteId);
        if (estudiante is null)
        {
            return BadRequest($"El estudiante con Id {dto.EstudianteId} no existe.");
        }

        var venta = new Venta
        {
            EstudianteId = dto.EstudianteId,
            Fecha = DateTime.Now,
            Total = 0
        };

        await _ventaRepository.AddAsync(venta);

        return CreatedAtAction(nameof(GetById), new { id = venta.Id }, ToResponse(venta));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VentaResponseDto>> Update(int id, VentaUpdateDto dto)
    {
        var venta = await _ventaRepository.GetByIdAsync(id);
        if (venta is null)
        {
            return NotFound();
        }

        var estudiante = await _estudianteRepository.GetByIdAsync(dto.EstudianteId);
        if (estudiante is null)
        {
            return BadRequest($"El estudiante con Id {dto.EstudianteId} no existe.");
        }

        venta.EstudianteId = dto.EstudianteId;
        await _ventaRepository.UpdateAsync(venta);

        return Ok(ToResponse(venta));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var venta = await _ventaRepository.GetByIdAsync(id);
        if (venta is null)
        {
            return NotFound();
        }

        // Los detalles de la venta se eliminan en cascada en la base de datos.
        await _ventaRepository.DeleteAsync(venta);

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