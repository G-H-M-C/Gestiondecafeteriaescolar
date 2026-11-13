using GestionCafeteriaEscolar.Domain.Entities;
using GestionCafeteriaEscolar.Infrastructure.Context;
using GestionCafeteriaEscolar.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace GestionCafeteriaEscolar.Infrastructure.Repositories;

public class EstudianteRepository : GenericRepository<Estudiante>
{
    public EstudianteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsMatriculaAsync(string matricula, int? excludeId = null)
    {
        return await _dbSet.AnyAsync(e => e.Matricula == matricula
            && (excludeId == null || e.Id != excludeId));
    }

    public async Task<bool> HasVentasAsync(int estudianteId)
    {
        return await _context.Ventas.AnyAsync(v => v.EstudianteId == estudianteId);
    }
}
