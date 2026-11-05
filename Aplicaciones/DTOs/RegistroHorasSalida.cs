namespace Aplicaciones.DTOs
{
    // Un turno del empleado (HoraSalida vacía si todavía está abierto).
    public class RegistroHorasSalida
    {
        public int Id { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly HoraEntrada { get; set; }
        public TimeOnly? HoraSalida { get; set; }
        public double HorasTrabajadas { get; set; }
    }
}
