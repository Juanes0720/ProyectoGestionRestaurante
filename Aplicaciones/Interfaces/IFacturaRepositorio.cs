using Modelos.Caja;

namespace Aplicaciones.Interfaces
{
    public interface IFacturaRepositorio
    {
        void Agregar(Factura factura);            // asigna el número consecutivo
        Factura? ObtenerPorNumero(int numero);
    }
}
