using GestionCafeteriaEscolar.Domain.Entities;
using GestionCafeteriaEscolar.Infrastructure.Context;
using GestionCafeteriaEscolar.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace GestionCafeteriaEscolar.Infrastructure.Repositories;

public class DetalleVentaRepository : GenericRepository<DetalleVenta>
{
    public DetalleVentaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> AnyByProductoIdAsync(int productoId)
    {
        return await _dbSet.AnyAsync(d => d.ProductoId == productoId);
    }

    public async Task<decimal> SumSubtotalByVentaAsync(int ventaId)
    {
        return await _dbSet
            .Where(d => d.VentaId == ventaId)
            .SumAsync(d => d.Subtotal);
    }
}
