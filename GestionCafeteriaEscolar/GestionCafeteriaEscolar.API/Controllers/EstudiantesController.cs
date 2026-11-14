using GestionCafeteriaEscolar.Application.Core;
using GestionCafeteriaEscolar.Application.Dtos;
using GestionCafeteriaEscolar.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstudiantesController : ControllerBase
{
	private readonly EstudianteService _estudianteService;

	public EstudiantesController(EstudianteService estudianteService)
	{
		_estudianteService = estudianteService;
	}

	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<EstudianteResponseDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<EstudianteResponseDto>>> GetAll()
	{
		var result = await _estudianteService.GetAllAsync();

		return Ok(result.Data);
	}

	[HttpGet("{id:int}")]
	[ProducesResponseType(typeof(EstudianteResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<EstudianteResponseDto>> GetById(int id)
	{
		var result = await _estudianteService.GetByIdAsync(id);
		if (result.Type == ResultType.NotFound)
		{
			return NotFound();
		}

		return Ok(result.Data);
	}

	[HttpPost]
	[ProducesResponseType(typeof(EstudianteResponseDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<EstudianteResponseDto>> Create(EstudianteCreateDto dto)
	{
		var result = await _estudianteService.CreateAsync(dto);
		if (result.Type == ResultType.Invalid)
		{
			return BadRequest(result.Error);
		}

		var estudiante = result.Data!;

		return CreatedAtAction(nameof(GetById), new { id = estudiante.Id }, estudiante);
	}

	[HttpPut("{id:int}")]
	[ProducesResponseType(typeof(EstudianteResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<EstudianteResponseDto>> Update(int id, EstudianteUpdateDto dto)
	{
		var result = await _estudianteService.UpdateAsync(id, dto);
		if (result.Type == ResultType.NotFound)
		{
			return NotFound();
		}

		if (result.Type == ResultType.Invalid)
		{
			return BadRequest(result.Error);
		}

		return Ok(result.Data);
	}

	[HttpDelete("{id:int}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete(int id)
	{
		var result = await _estudianteService.DeleteAsync(id);
		if (result.Type == ResultType.NotFound)
		{
			return NotFound();
		}

		if (result.Type == ResultType.Invalid)
		{
			return BadRequest(result.Error);
		}

		return NoContent();
	}
}