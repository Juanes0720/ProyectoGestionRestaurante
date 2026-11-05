using Modelos.Gastos;

namespace Aplicaciones.Interfaces
{
    public interface IGastoRepositorio
    {
        void Agregar(Gasto gasto);                    // asigna el Id
        Gasto? ObtenerPorId(int id);
        List<Gasto> ObtenerPorFecha(DateOnly fecha);
    }
}
