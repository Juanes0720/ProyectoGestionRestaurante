using Modelos.Pedidos;

namespace Aplicaciones.Interfaces
{
    public interface IPedidoRepositorio
    {
        void Agregar(Pedido pedido);              // asigna el Id
        Pedido? ObtenerPorId(int id);
        List<Pedido> ObtenerTodos();
    }
}
