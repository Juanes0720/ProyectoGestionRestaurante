namespace Modelos.Pedidos
{
    public class PedidoComerLocal : Pedido
    {
        public int Mesa { get; set; }

        public override string Tipo => "ComerLocal";
    }
}
