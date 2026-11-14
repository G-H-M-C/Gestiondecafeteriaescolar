using GestionCafeteriaEscolar.Application.Core;
using GestionCafeteriaEscolar.Application.Dtos;
using GestionCafeteriaEscolar.Domain.Entities;
using GestionCafeteriaEscolar.Infrastructure.Core;
using GestionCafeteriaEscolar.Infrastructure.Repositories;

namespace GestionCafeteriaEscolar.Application.Services;

public class ProductoService
{
    private readonly GenericRepository<Producto> _productoRepository;
    private readonly DetalleVentaRepository _detalleVentaRepository;

    public ProductoService(
        GenericRepository<Producto> productoRepository,
        DetalleVentaRepository detalleVentaRepository)
    {
        _productoRepository = productoRepository;
        _detalleVentaRepository = detalleVentaRepository;
    }

    public async Task<ServiceResult<IEnumerable<ProductoResponseDto>>> GetAllAsync()
    {
        var productos = await _productoRepository.GetAllAsync();

        return ServiceResult<IEnumerable<ProductoResponseDto>>.Ok(productos.Select(ToResponse));
    }

    public async Task<ServiceResult<ProductoResponseDto>> GetByIdAsync(int id)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto is null)
        {
            return ServiceResult<ProductoResponseDto>.NotFound();
        }

        return ServiceResult<ProductoResponseDto>.Ok(ToResponse(producto));
    }

    public async Task<ServiceResult<ProductoResponseDto>> CreateAsync(ProductoCreateDto dto)
    {
        var error = Validar(dto.Nombre, dto.Precio, dto.Stock);
        if (error is not null)
        {
            return ServiceResult<ProductoResponseDto>.Invalid(error);
        }

        var producto = new Producto
        {
            Nombre = dto.Nombre.Trim(),
            Precio = dto.Precio,
            Stock = dto.Stock
        };

        await _productoRepository.AddAsync(producto);

        return ServiceResult<ProductoResponseDto>.Created(ToResponse(producto));
    }

    public async Task<ServiceResult<ProductoResponseDto>> UpdateAsync(int id, ProductoUpdateDto dto)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto is null)
        {
            return ServiceResult<ProductoResponseDto>.NotFound();
        }

        var error = Validar(dto.Nombre, dto.Precio, dto.Stock);
        if (error is not null)
        {
            return ServiceResult<ProductoResponseDto>.Invalid(error);
        }

        producto.Nombre = dto.Nombre.Trim();
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;
        await _productoRepository.UpdateAsync(producto);

        return ServiceResult<ProductoResponseDto>.Ok(ToResponse(producto));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto is null)
        {
            return ServiceResult<bool>.NotFound();
        }

        var tieneDetalles = await _detalleVentaRepository.AnyByProductoIdAsync(id);
        if (tieneDetalles)
        {
            return ServiceResult<bool>.Invalid("No se puede eliminar el producto porque tiene ventas registradas.");
        }

        await _productoRepository.DeleteAsync(producto);

        return ServiceResult<bool>.NoContent();
    }

    private static string? Validar(string nombre, decimal precio, int stock)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return "El nombre es obligatorio.";
        }

        var largo = nombre.Trim().Length;
        if (largo < 2 || largo > 100)
        {
            return "El nombre debe tener entre 2 y 100 caracteres.";
        }

        if (precio <= 0)
        {
            return "El precio debe ser mayor que 0.";
        }

        if (precio > 100000)
        {
            return "El precio no puede superar 100000.";
        }

        if (stock < 0)
        {
            return "El stock no puede ser negativo.";
        }

        return null;
    }

    private static ProductoResponseDto ToResponse(Producto producto)
    {
        return new ProductoResponseDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Stock = producto.Stock
        };
    }
}
