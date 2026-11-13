using GestionCafeteriaEscolar.Domain.Core;

namespace GestionCafeteriaEscolar.Domain.Entities;

public class Estudiante : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string Grado { get; set; } = string.Empty;
}