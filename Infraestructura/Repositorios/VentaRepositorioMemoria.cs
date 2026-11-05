using Aplicaciones.Interfaces;
using Modelos.Ventas;

namespace Infraestructura.Repositorios
{
    public class VentaRepositorioMemoria : IVentaRepositorio
    {
        private readonly List<Venta> _ventas = new();
        private int _siguienteId = 1;

        public void Agregar(Venta venta)
        {
            venta.Id = _siguienteId++;
            _ventas.Add(venta);
        }

        public Venta? ObtenerPorId(int id)
        {
            return _ventas.FirstOrDefault(v => v.Id == id);
        }
    }
}
