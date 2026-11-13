using GestionCafeteriaEscolar.Domain.Core;

namespace GestionCafeteriaEscolar.Domain.Entities;

public class Venta : BaseEntity
{
    public int EstudianteId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
}
