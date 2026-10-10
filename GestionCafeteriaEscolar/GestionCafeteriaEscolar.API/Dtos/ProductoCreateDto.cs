using System.ComponentModel.DataAnnotations;

namespace GestionCafeteriaEscolar.API.Dtos;

public class ProductoCreateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2)]
    public string Nombre { get; set; } = string.Empty;

    [Range(0.01, 100000, ErrorMessage = "El precio debe ser mayor que 0.")]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }
}