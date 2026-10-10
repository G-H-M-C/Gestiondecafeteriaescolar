namespace GestionCafeteriaEscolar.API.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Grado { get; set; } = string.Empty;
    }
}
