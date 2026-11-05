namespace Aplicaciones.DTOs
{
    public class DetalleVentaSalida
    {
        public int? ProductoId { get; set; }      // vacío en conceptos que no son productos (envío)
        public string Descripcion { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
