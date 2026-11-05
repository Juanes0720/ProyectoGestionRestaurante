namespace Aplicaciones.DTOs
{
    public class PedidoSalida
    {
        public int Id { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly HoraPedido { get; set; }   // la pone el sistema al crear el pedido
        public string Tipo { get; set; } = string.Empty;
        public List<DetalleVentaSalida> Detalles { get; set; } = new();
        public decimal Total { get; set; }
        public int VentaId { get; set; }
        public bool Cobrado { get; set; }
        public int? FacturaNumero { get; set; }

        // Datos propios de cada tipo (vienen vacíos si no aplican)
        public int? Mesa { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public decimal? CostoEnvio { get; set; }
        public string? NombreCliente { get; set; }
    }
}
