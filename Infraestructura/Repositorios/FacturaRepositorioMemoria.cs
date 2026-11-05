using Aplicaciones.Interfaces;
using Modelos.Caja;

namespace Infraestructura.Repositorios
{
    public class FacturaRepositorioMemoria : IFacturaRepositorio
    {
        private readonly List<Factura> _facturas = new();
        private int _siguienteNumero = 1;    // consecutivo

        public void Agregar(Factura factura)
        {
            factura.Numero = _siguienteNumero++;
            _facturas.Add(factura);
        }

        public Factura? ObtenerPorNumero(int numero)
        {
            return _facturas.FirstOrDefault(f => f.Numero == numero);
        }
    }
}
