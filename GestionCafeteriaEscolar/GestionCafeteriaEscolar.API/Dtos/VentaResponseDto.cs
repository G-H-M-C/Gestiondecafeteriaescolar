namespace GestionCafeteriaEscolar.API.Dtos;

public class VentaResponseDto
{
    public int Id { get; set; }
    public int EstudianteId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
}