using Aplicaciones.Interfaces;
using Modelos.Caja;

namespace Infraestructura.Repositorios
{
    public class AperturaCajaRepositorioMemoria : IAperturaCajaRepositorio
    {
        private readonly List<AperturaCaja> _aperturas = new();
        private int _siguienteId = 1;

        public void Agregar(AperturaCaja apertura)
        {
            apertura.Id = _siguienteId++;
            _aperturas.Add(apertura);
        }

        public AperturaCaja? ObtenerAbierta()
        {
            return _aperturas.FirstOrDefault(a => a.Abierta);
        }
    }
}
