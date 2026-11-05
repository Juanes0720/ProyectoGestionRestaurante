using Aplicaciones.Interfaces;
using Modelos.Pedidos;

namespace Infraestructura.Repositorios
{
    public class PedidoRepositorioMemoria : IPedidoRepositorio
    {
        private readonly List<Pedido> _pedidos = new();
        private int _siguienteId = 1;

        public void Agregar(Pedido pedido)
        {
            pedido.Id = _siguienteId++;
            _pedidos.Add(pedido);
        }

        public Pedido? ObtenerPorId(int id)
        {
            return _pedidos.FirstOrDefault(p => p.Id == id);
        }

        public List<Pedido> ObtenerTodos()
        {
            return _pedidos.ToList();
        }
    }
}
