using Aplicaciones.DTOs;

namespace Aplicaciones.Interfaces
{
    public interface ICorteRepositorio
    {
        void Agregar(CorteSalida corte);
        List<CorteSalida> ObtenerTodos();
    }
}
