using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;

namespace Infraestructura.Repositorios
{
    public class CorteRepositorioMemoria : ICorteRepositorio
    {
        private readonly List<CorteSalida> _cortes = new();

        public void Agregar(CorteSalida corte)
        {
            _cortes.Add(corte);
        }

        public List<CorteSalida> ObtenerTodos()
        {
            return _cortes.ToList();
        }
    }
}
