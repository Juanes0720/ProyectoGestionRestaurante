namespace Aplicaciones.DTOs
{
    // Resumen del turno. No se ingresa nada: todo se calcula desde la apertura de caja.
    public class CorteSalida
    {
        public int AperturaId { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly HoraInicioTurno { get; set; }
        public TimeOnly HoraCorte { get; set; }
        public bool CajaCerrada { get; set; }
        public decimal BaseInicial { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal TotalVentas { get; set; }     // dinero que se hizo en el día
        public decimal TotalEsperado { get; set; }   // base inicial + ventas - gastos
    }
}
