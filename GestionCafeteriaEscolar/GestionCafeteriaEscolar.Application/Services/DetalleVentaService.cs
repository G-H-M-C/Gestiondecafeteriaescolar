using GestionCafeteriaEscolar.Application.Core;
using GestionCafeteriaEscolar.Application.Dtos;
using GestionCafeteriaEscolar.Domain.Entities;
using GestionCafeteriaEscolar.Infrastructure.Core;
using GestionCafeteriaEscolar.Infrastructure.Repositories;

namespace GestionCafeteriaEscolar.Application.Services;

public class DetalleVentaService
{
    private readonly DetalleVentaRepository _detalleRepository;
    private readonly GenericRepository<Venta> _ventaRepository;
    private readonly GenericRepository<Producto> _productoRepository;

    public DetalleVentaService(
        DetalleVentaRepository detalleRepository,
        GenericRepository<Venta> ventaRepository,
        GenericRepository<Producto> productoRepository)
    {
        _detalleRepository = detalleRepository;
        _ventaRepository = ventaRepository;
        _productoRepository = productoRepository;
    }

    public async Task<ServiceResult<IEnumerable<DetalleVentaResponseDto>>> GetAllAsync()
    {
        var detalles = await _detalleRepository.GetAllAsync();

        return ServiceResult<IEnumerable<DetalleVentaResponseDto>>.Ok(detalles.Select(ToResponse));
    }

    public async Task<ServiceResult<DetalleVentaResponseDto>> GetByIdAsync(int id)
    {
        var detalle = await _detalleRepository.GetByIdAsync(id);
        if (detalle is null)
        {
            return ServiceResult<DetalleVentaResponseDto>.NotFound();
        }

        return ServiceResult<DetalleVentaResponseDto>.Ok(ToResponse(detalle));
    }

    public async Task<ServiceResult<DetalleVentaResponseDto>> CreateAsync(DetalleVentaCreateDto dto)
    {
        var error = Validar(dto.VentaId, dto.ProductoId, dto.Cantidad);
        if (error is not null)
        {
            return ServiceResult<DetalleVentaResponseDto>.Invalid(error);
        }

        var venta = await _ventaRepository.GetByIdAsync(dto.VentaId);
        if (venta is null)
        {
            return ServiceResult<DetalleVentaResponseDto>.Invalid($"La venta con Id {dto.VentaId} no existe.");
        }

        var producto = await _productoRepository.GetByIdAsync(dto.ProductoId);
        if (producto is null)
        {
            return ServiceResult<DetalleVentaResponseDto>.Invalid($"El producto con Id {dto.ProductoId} no existe.");
        }

        var detalle = new DetalleVenta
        {
            VentaId = dto.VentaId,
            ProductoId = dto.ProductoId,
            Cantidad = dto.Cantidad,
            PrecioUnitario = producto.Precio,
            Subtotal = producto.Precio * dto.Cantidad
        };

        await _detalleRepository.AddAsync(detalle);
        await RecalcularTotalAsync(detalle.VentaId);

        return ServiceResult<DetalleVentaResponseDto>.Created(ToResponse(detalle));
    }

    public async Task<ServiceResult<DetalleVentaResponseDto>> UpdateAsync(int id, DetalleVentaUpdateDto dto)
    {
        var detalle = await _detalleRepository.GetByIdAsync(id);
        if (detalle is null)
        {
            return ServiceResult<DetalleVentaResponseDto>.NotFound();
        }

        var error = Validar(dto.VentaId, dto.ProductoId, dto.Cantidad);
        if (error is not null)
        {
            return ServiceResult<DetalleVentaResponseDto>.Invalid(error);
        }

        var venta = await _ventaRepository.GetByIdAsync(dto.VentaId);
        if (venta is null)
        {
            return ServiceResult<DetalleVentaResponseDto>.Invalid($"La venta con Id {dto.VentaId} no existe.");
        }

        var producto = await _productoRepository.GetByIdAsync(dto.ProductoId);
        if (producto is null)
        {
            return ServiceResult<DetalleVentaResponseDto>.Invalid($"El producto con Id {dto.ProductoId} no existe.");
        }

        var ventaAnteriorId = detalle.VentaId;

        detalle.VentaId = dto.VentaId;
        detalle.ProductoId = dto.ProductoId;
        detalle.Cantidad = dto.Cantidad;
        detalle.PrecioUnitario = producto.Precio;
        detalle.Subtotal = producto.Precio * dto.Cantidad;
        await _detalleRepository.UpdateAsync(detalle);

        await RecalcularTotalAsync(detalle.VentaId);
        if (ventaAnteriorId != detalle.VentaId)
        {
            await RecalcularTotalAsync(ventaAnteriorId);
        }

        return ServiceResult<DetalleVentaResponseDto>.Ok(ToResponse(detalle));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var detalle = await _detalleRepository.GetByIdAsync(id);
        if (detalle is null)
        {
            return ServiceResult<bool>.NotFound();
        }

        var ventaId = detalle.VentaId;

        await _detalleRepository.DeleteAsync(detalle);
        await RecalcularTotalAsync(ventaId);

        return ServiceResult<bool>.NoContent();
    }

    private async Task RecalcularTotalAsync(int ventaId)
    {
        var venta = await _ventaRepository.GetByIdAsync(ventaId);
        if (venta is null)
        {
            return;
        }

        venta.Total = await _detalleRepository.SumSubtotalByVentaAsync(ventaId);
        await _ventaRepository.UpdateAsync(venta);
    }

    private static string? Validar(int ventaId, int productoId, int cantidad)
    {
        if (ventaId <= 0)
        {
            return "El Id de la venta debe ser mayor que 0.";
        }

        if (productoId <= 0)
        {
            return "El Id del producto debe ser mayor que 0.";
        }

        if (cantidad <= 0)
        {
            return "La cantidad debe ser mayor que 0.";
        }

        return null;
    }

    private static DetalleVentaResponseDto ToResponse(DetalleVenta detalle)
    {
        return new DetalleVentaResponseDto
        {
            Id = detalle.Id,
            VentaId = detalle.VentaId,
            ProductoId = detalle.ProductoId,
            Cantidad = detalle.Cantidad,
            PrecioUnitario = detalle.PrecioUnitario,
            Subtotal = detalle.Subtotal
        };
    }
}
