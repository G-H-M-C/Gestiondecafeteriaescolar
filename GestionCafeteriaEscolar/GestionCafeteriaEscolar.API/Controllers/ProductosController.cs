using GestionCafeteriaEscolar.Application.Core;
using GestionCafeteriaEscolar.Application.Dtos;
using GestionCafeteriaEscolar.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionCafeteriaEscolar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
	private readonly ProductoService _productoService;

	public ProductosController(ProductoService productoService)
	{
		_productoService = productoService;
	}

	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<ProductoResponseDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetAll()
	{
		var result = await _productoService.GetAllAsync();

		return Ok(result.Data);
	}

	[HttpGet("{id:int}")]
	[ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ProductoResponseDto>> GetById(int id)
	{
		var result = await _productoService.GetByIdAsync(id);
		if (result.Type == ResultType.NotFound)
		{
			return NotFound();
		}

		return Ok(result.Data);
	}

	[HttpPost]
	[ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<ProductoResponseDto>> Create(ProductoCreateDto dto)
	{
		var result = await _productoService.CreateAsync(dto);
		if (result.Type == ResultType.Invalid)
		{
			return BadRequest(result.Error);
		}

		var producto = result.Data!;

		return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
	}

	[HttpPut("{id:int}")]
	[ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ProductoResponseDto>> Update(int id, ProductoUpdateDto dto)
	{
		var result = await _productoService.UpdateAsync(id, dto);
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
		var result = await _productoService.DeleteAsync(id);
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