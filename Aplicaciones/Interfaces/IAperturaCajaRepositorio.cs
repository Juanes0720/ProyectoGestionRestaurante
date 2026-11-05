using Modelos.Caja;

namespace Aplicaciones.Interfaces
{
    public interface IAperturaCajaRepositorio
    {
        void Agregar(AperturaCaja apertura);      // asigna el Id
        AperturaCaja? ObtenerAbierta();           // la única apertura abierta (o null)
    }
}
