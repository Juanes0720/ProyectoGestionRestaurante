namespace Aplicaciones.DTOs
{
    public class VentaSalida
    {
        public int Id { get; set; }
        public List<DetalleVentaSalida> Detalles { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public bool Cobrada { get; set; }
    }
}
