using Modelos.Ventas;

namespace Aplicaciones.Interfaces
{
    public interface IVentaRepositorio
    {
        void Agregar(Venta venta);                // asigna el Id
        Venta? ObtenerPorId(int id);
    }
}
