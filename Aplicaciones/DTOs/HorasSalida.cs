namespace Aplicaciones.DTOs
{
    public class HorasSalida
    {
        public int EmpleadoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public double HorasTrabajadas { get; set; }
        public int RegistrosCerrados { get; set; }
        public bool TieneEntradaAbierta { get; set; }
        public List<RegistroHorasSalida> Registros { get; set; } = new();
    }
}
