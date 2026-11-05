namespace Modelos.Empleados
{
    public class RegistroHoras
    {
        public int Id { get; set; }
        public Empleado Empleado { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly HoraEntrada { get; set; }

        // Vacía (null) mientras el registro esté abierto.
        public TimeOnly? HoraSalida { get; private set; }

        // Se calcula y se guarda al cerrar el turno (0 mientras esté abierto).
        public double HorasTrabajadas { get; private set; }

        public bool EstaAbierto => HoraSalida == null;

        public RegistroHoras(Empleado empleado, DateOnly fecha, TimeOnly horaEntrada)
        {
            Empleado = empleado;
            Fecha = fecha;
            HoraEntrada = horaEntrada;
        }

        // Cierra el turno: guarda la hora de salida y las horas trabajadas.
        public void CerrarTurno(TimeOnly horaSalida)
        {
            HoraSalida = horaSalida;
            HorasTrabajadas = Math.Round((horaSalida - HoraEntrada).TotalHours, 2);
        }
    }
}
