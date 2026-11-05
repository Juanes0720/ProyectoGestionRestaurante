using Aplicaciones.Interfaces;
using Modelos.Ventas;

namespace Infraestructura.Repositorios
{
    public class ProductoRepositorioMemoria : IProductoRepositorio
    {
        private readonly List<Producto> _productos = new();
        private int _siguienteId = 1;

        public void Agregar(Producto producto)
        {
            producto.Id = _siguienteId++;
            _productos.Add(producto);
        }

        public Producto? ObtenerPorId(int id)
        {
            return _productos.FirstOrDefault(p => p.Id == id);
        }

        public List<Producto> ObtenerTodos()
        {
            return _productos.ToList();
        }
    }
}
