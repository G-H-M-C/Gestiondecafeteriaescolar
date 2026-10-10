namespace GestionCafeteriaEscolar.API.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
    }
}
