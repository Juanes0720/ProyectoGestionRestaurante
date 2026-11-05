namespace Modelos.Pedidos
{
    public class PedidoDomicilio : Pedido
    {
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public decimal CostoEnvio { get; set; }

        public override string Tipo => "Domicilio";

        // El envío se suma al total del pedido.
        public override decimal Total => base.Total + CostoEnvio;
    }
}
