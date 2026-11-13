using GestionCafeteriaEscolar.Domain.Core;

namespace GestionCafeteriaEscolar.Domain.Entities;

public class DetalleVenta : BaseEntity
{
    public int VentaId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}