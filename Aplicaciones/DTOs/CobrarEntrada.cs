namespace Aplicaciones.DTOs
{
    // El cajero es quien inició sesión (sale del token).
    public class CobrarEntrada
    {
        public int VentaId { get; set; }
        public decimal DineroRecibido { get; set; }
    }
}
