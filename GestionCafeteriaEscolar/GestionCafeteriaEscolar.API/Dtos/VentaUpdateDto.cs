using System.ComponentModel.DataAnnotations;

namespace GestionCafeteriaEscolar.API.Dtos;

public class VentaUpdateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El EstudianteId debe ser mayor que 0.")]
    public int EstudianteId { get; set; }
}