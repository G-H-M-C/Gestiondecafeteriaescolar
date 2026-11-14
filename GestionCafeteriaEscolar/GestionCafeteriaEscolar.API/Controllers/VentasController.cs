using GestionCafeteriaEscolar.Application.Core;
using GestionCafeteriaEscolar.Application.Dtos;
using GestionCafeteriaEscolar.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
	private readonly VentaService _ventaService;

	public VentasController(VentaService ventaService)
	{
		_ventaService = ventaService;
	}

	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<VentaResponseDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<VentaResponseDto>>> GetAll()
	{
		var result = await _ventaService.GetAllAsync();

		return Ok(result.Data);
	}

	[HttpGet("{id:int}")]
	[ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<VentaResponseDto>> GetById(int id)
	{
		var result = await _ventaService.GetByIdAsync(id);
		if (result.Type == ResultType.NotFound)
		{
			return NotFound();
		}

		return Ok(result.Data);
	}

	[HttpPost]
	[ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<VentaResponseDto>> Create(VentaCreateDto dto)
	{
		var result = await _ventaService.CreateAsync(dto);
		if (result.Type == ResultType.Invalid)
		{
			return BadRequest(result.Error);
		}

		var venta = result.Data!;

		return CreatedAtAction(nameof(GetById), new { id = venta.Id }, venta);
	}

	[HttpPut("{id:int}")]
	[ProducesResponseType(typeof(VentaResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<VentaResponseDto>> Update(int id, VentaUpdateDto dto)
	{
		var result = await _ventaService.UpdateAsync(id, dto);
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
		var result = await _ventaService.DeleteAsync(id);
		if (result.Type == ResultType.NotFound)
		{
			return NotFound();
		}

		return NoContent();
	}
}