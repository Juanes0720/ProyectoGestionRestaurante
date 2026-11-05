namespace Aplicaciones.DTOs
{
    // Un solo DTO para los tres tipos. Cada tipo usa solo sus campos.
    public class PedidoEntrada
    {
        // "ComerLocal", "Domicilio" o "RecogerTienda"
        public string Tipo { get; set; } = string.Empty;

        // Los productos del pedido
        public List<ItemVentaEntrada> Items { get; set; } = new();

        // ComerLocal
        public int? Mesa { get; set; }

        // Domicilio
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public decimal CostoEnvio { get; set; }

        // RecogerTienda
        public string? NombreCliente { get; set; }
    }
}
