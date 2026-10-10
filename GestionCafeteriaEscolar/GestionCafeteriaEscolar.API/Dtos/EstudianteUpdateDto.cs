using System.ComponentModel.DataAnnotations;

namespace GestionCafeteriaEscolar.API.Dtos;

public class EstudianteUpdateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La matrícula es obligatoria.")]
    [StringLength(20, MinimumLength = 3)]
    public string Matricula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El grado es obligatorio.")]
    [StringLength(30)]
    public string Grado { get; set; } = string.Empty;
}
