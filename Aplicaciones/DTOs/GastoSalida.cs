namespace Aplicaciones.DTOs
{
    public class GastoSalida
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly Hora { get; set; }
        public string Empleado { get; set; } = string.Empty;
    }
}
