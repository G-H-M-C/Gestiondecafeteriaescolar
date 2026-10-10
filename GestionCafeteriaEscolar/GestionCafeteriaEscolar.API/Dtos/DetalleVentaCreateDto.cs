using System.ComponentModel.DataAnnotations;

namespace GestionCafeteriaEscolar.API.Dtos;

public class DetalleVentaCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El VentaId debe ser mayor que 0.")]
    public int VentaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El ProductoId debe ser mayor que 0.")]
    public int ProductoId { get; set; }

    [Range(1, 1000, ErrorMessage = "La cantidad debe estar entre 1 y 1000.")]
    public int Cantidad { get; set; }
}