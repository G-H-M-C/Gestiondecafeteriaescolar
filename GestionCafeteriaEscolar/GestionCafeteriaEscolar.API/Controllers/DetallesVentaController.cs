using GestionCafeteriaEscolar.Application.Core;
using GestionCafeteriaEscolar.Application.Dtos;
using GestionCafeteriaEscolar.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetallesVentaController : ControllerBase
{
	private readonly DetalleVentaService _detalleVentaService;

	public DetallesVentaController(DetalleVentaService detalleVentaService)
	{
		_detalleVentaService = detalleVentaService;
	}

	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<DetalleVentaResponseDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<DetalleVentaResponseDto>>> GetAll()
	{
		var result = await _detalleVentaService.GetAllAsync();

		return Ok(result.Data);
	}

	[HttpGet("{id:int}")]
	[ProducesResponseType(typeof(DetalleVentaResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<DetalleVentaResponseDto>> GetById(int id)
	{
		var result = await _detalleVentaService.GetByIdAsync(id);
		if (result.Type == ResultType.NotFound)
		{
			return NotFound();
		}

		return Ok(result.Data);
	}

	[HttpPost]
	[ProducesResponseType(typeof(DetalleVentaResponseDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<DetalleVentaResponseDto>> Create(DetalleVentaCreateDto dto)
	{
		var result = await _detalleVentaService.CreateAsync(dto);
		if (result.Type == ResultType.Invalid)
		{
			return BadRequest(result.Error);
		}

		var detalle = result.Data!;

		return CreatedAtAction(nameof(GetById), new { id = detalle.Id }, detalle);
	}

	[HttpPut("{id:int}")]
	[ProducesResponseType(typeof(DetalleVentaResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<DetalleVentaResponseDto>> Update(int id, DetalleVentaUpdateDto dto)
	{
		var result = await _detalleVentaService.UpdateAsync(id, dto);
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
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete(int id)
	{
		var result = await _detalleVentaService.DeleteAsync(id);
		if (result.Type == ResultType.NotFound)
		{
			return NotFound();
		}

		return NoContent();
	}
}