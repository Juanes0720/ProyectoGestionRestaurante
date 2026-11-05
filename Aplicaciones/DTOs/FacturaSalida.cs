namespace Aplicaciones.DTOs
{
    public class FacturaSalida
    {
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Cajero { get; set; } = string.Empty;
        public int VentaId { get; set; }
        public int? PedidoId { get; set; }
        public DateOnly? FechaPedido { get; set; }
        public TimeOnly? HoraPedido { get; set; }
        public string TipoPedido { get; set; } = string.Empty;
        public List<DetalleVentaSalida> Detalles { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public decimal ValorPagado { get; set; }
        public decimal Cambio { get; set; }
    }
}
