using Aplicaciones.Interfaces;
using Modelos.Gastos;

namespace Infraestructura.Repositorios
{
    public class GastoRepositorioMemoria : IGastoRepositorio
    {
        private readonly List<Gasto> _gastos = new();
        private int _siguienteId = 1;

        public void Agregar(Gasto gasto)
        {
            gasto.Id = _siguienteId++;
            _gastos.Add(gasto);
        }

        public Gasto? ObtenerPorId(int id)
        {
            return _gastos.FirstOrDefault(g => g.Id == id);
        }

        public List<Gasto> ObtenerPorFecha(DateOnly fecha)
        {
            return _gastos.Where(g => DateOnly.FromDateTime(g.Fecha) == fecha).ToList();
        }
    }
}
