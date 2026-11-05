namespace Modelos.Pedidos
{
    public class PedidoRecogerTienda : Pedido
    {
        public string NombreCliente { get; set; } = string.Empty;

        public override string Tipo => "RecogerTienda";
    }
}
