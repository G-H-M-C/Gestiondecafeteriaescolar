using GestionCafeteriaEscolar.Application.Core;
using GestionCafeteriaEscolar.Application.Dtos;
using GestionCafeteriaEscolar.Domain.Entities;
using GestionCafeteriaEscolar.Infrastructure.Repositories;

namespace GestionCafeteriaEscolar.Application.Services;

public class EstudianteService
{
    private readonly EstudianteRepository _estudianteRepository;

    public EstudianteService(EstudianteRepository estudianteRepository)
    {
        _estudianteRepository = estudianteRepository;
    }

    public async Task<ServiceResult<IEnumerable<EstudianteResponseDto>>> GetAllAsync()
    {
        var estudiantes = await _estudianteRepository.GetAllAsync();

        return ServiceResult<IEnumerable<EstudianteResponseDto>>.Ok(estudiantes.Select(ToResponse));
    }

    public async Task<ServiceResult<EstudianteResponseDto>> GetByIdAsync(int id)
    {
        var estudiante = await _estudianteRepository.GetByIdAsync(id);
        if (estudiante is null)
        {
            return ServiceResult<EstudianteResponseDto>.NotFound();
        }

        return ServiceResult<EstudianteResponseDto>.Ok(ToResponse(estudiante));
    }

    public async Task<ServiceResult<EstudianteResponseDto>> CreateAsync(EstudianteCreateDto dto)
    {
        var error = Validar(dto.Nombre, dto.Matricula, dto.Grado);
        if (error is not null)
        {
            return ServiceResult<EstudianteResponseDto>.Invalid(error);
        }

        var matricula = dto.Matricula.Trim();
        var matriculaExiste = await _estudianteRepository.ExistsMatriculaAsync(matricula);
        if (matriculaExiste)
        {
            return ServiceResult<EstudianteResponseDto>.Invalid($"Ya existe un estudiante con la matrícula {matricula}.");
        }

        var estudiante = new Estudiante
        {
            Nombre = dto.Nombre.Trim(),
            Matricula = matricula,
            Grado = dto.Grado.Trim()
        };

        await _estudianteRepository.AddAsync(estudiante);

        return ServiceResult<EstudianteResponseDto>.Created(ToResponse(estudiante));
    }

    public async Task<ServiceResult<EstudianteResponseDto>> UpdateAsync(int id, EstudianteUpdateDto dto)
    {
        var estudiante = await _estudianteRepository.GetByIdAsync(id);
        if (estudiante is null)
        {
            return ServiceResult<EstudianteResponseDto>.NotFound();
        }

        var error = Validar(dto.Nombre, dto.Matricula, dto.Grado);
        if (error is not null)
        {
            return ServiceResult<EstudianteResponseDto>.Invalid(error);
        }

        var matricula = dto.Matricula.Trim();
        var matriculaExiste = await _estudianteRepository.ExistsMatriculaAsync(matricula, id);
        if (matriculaExiste)
        {
            return ServiceResult<EstudianteResponseDto>.Invalid($"Ya existe un estudiante con la matrícula {matricula}.");
        }

        estudiante.Nombre = dto.Nombre.Trim();
        estudiante.Matricula = matricula;
        estudiante.Grado = dto.Grado.Trim();
        await _estudianteRepository.UpdateAsync(estudiante);

        return ServiceResult<EstudianteResponseDto>.Ok(ToResponse(estudiante));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var estudiante = await _estudianteRepository.GetByIdAsync(id);
        if (estudiante is null)
        {
            return ServiceResult<bool>.NotFound();
        }

        var tieneVentas = await _estudianteRepository.HasVentasAsync(id);
        if (tieneVentas)
        {
            return ServiceResult<bool>.Invalid("No se puede eliminar el estudiante porque tiene ventas registradas.");
        }

        await _estudianteRepository.DeleteAsync(estudiante);

        return ServiceResult<bool>.NoContent();
    }

    private static string? Validar(string nombre, string matricula, string grado)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return "El nombre es obligatorio.";
        }

        var largoNombre = nombre.Trim().Length;
        if (largoNombre < 2 || largoNombre > 100)
        {
            return "El nombre debe tener entre 2 y 100 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(matricula))
        {
            return "La matrícula es obligatoria.";
        }

        var largoMatricula = matricula.Trim().Length;
        if (largoMatricula < 3 || largoMatricula > 20)
        {
            return "La matrícula debe tener entre 3 y 20 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(grado))
        {
            return "El grado es obligatorio.";
        }

        if (grado.Trim().Length > 30)
        {
            return "El grado no puede tener más de 30 caracteres.";
        }

        return null;
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
