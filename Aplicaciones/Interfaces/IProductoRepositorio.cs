using Modelos.Ventas;

namespace Aplicaciones.Interfaces
{
    public interface IProductoRepositorio
    {
        void Agregar(Producto producto);          // asigna el Id
        Producto? ObtenerPorId(int id);
        List<Producto> ObtenerTodos();
    }
}
