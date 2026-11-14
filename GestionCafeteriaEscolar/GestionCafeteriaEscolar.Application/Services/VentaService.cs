using GestionCafeteriaEscolar.Application.Core;
using GestionCafeteriaEscolar.Application.Dtos;
using GestionCafeteriaEscolar.Domain.Entities;
using GestionCafeteriaEscolar.Infrastructure.Core;
using GestionCafeteriaEscolar.Infrastructure.Repositories;

namespace GestionCafeteriaEscolar.Application.Services;

public class VentaService
{
    private readonly GenericRepository<Venta> _ventaRepository;
    private readonly EstudianteRepository _estudianteRepository;

    public VentaService(
        GenericRepository<Venta> ventaRepository,
        EstudianteRepository estudianteRepository)
    {
        _ventaRepository = ventaRepository;
        _estudianteRepository = estudianteRepository;
    }

    public async Task<ServiceResult<IEnumerable<VentaResponseDto>>> GetAllAsync()
    {
        var ventas = await _ventaRepository.GetAllAsync();

        return ServiceResult<IEnumerable<VentaResponseDto>>.Ok(ventas.Select(ToResponse));
    }

    public async Task<ServiceResult<VentaResponseDto>> GetByIdAsync(int id)
    {
        var venta = await _ventaRepository.GetByIdAsync(id);
        if (venta is null)
        {
            return ServiceResult<VentaResponseDto>.NotFound();
        }

        return ServiceResult<VentaResponseDto>.Ok(ToResponse(venta));
    }

    public async Task<ServiceResult<VentaResponseDto>> CreateAsync(VentaCreateDto dto)
    {
        var error = await ValidarEstudianteAsync(dto.EstudianteId);
        if (error is not null)
        {
            return ServiceResult<VentaResponseDto>.Invalid(error);
        }

        var venta = new Venta
        {
            EstudianteId = dto.EstudianteId,
            Fecha = DateTime.Now,
            Total = 0
        };

        await _ventaRepository.AddAsync(venta);

        return ServiceResult<VentaResponseDto>.Created(ToResponse(venta));
    }

    public async Task<ServiceResult<VentaResponseDto>> UpdateAsync(int id, VentaUpdateDto dto)
    {
        var venta = await _ventaRepository.GetByIdAsync(id);
        if (venta is null)
        {
            return ServiceResult<VentaResponseDto>.NotFound();
        }

        var error = await ValidarEstudianteAsync(dto.EstudianteId);
        if (error is not null)
        {
            return ServiceResult<VentaResponseDto>.Invalid(error);
        }

        venta.EstudianteId = dto.EstudianteId;
        await _ventaRepository.UpdateAsync(venta);

        return ServiceResult<VentaResponseDto>.Ok(ToResponse(venta));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var venta = await _ventaRepository.GetByIdAsync(id);
        if (venta is null)
        {
            return ServiceResult<bool>.NotFound();
        }

        // Los detalles de la venta se eliminan en cascada en la base de datos.
        await _ventaRepository.DeleteAsync(venta);

        return ServiceResult<bool>.NoContent();
    }

    private async Task<string?> ValidarEstudianteAsync(int estudianteId)
    {
        if (estudianteId <= 0)
        {
            return "El Id del estudiante debe ser mayor que 0.";
        }

        var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId);
        if (estudiante is null)
        {
            return $"El estudiante con Id {estudianteId} no existe.";
        }

        return null;
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
