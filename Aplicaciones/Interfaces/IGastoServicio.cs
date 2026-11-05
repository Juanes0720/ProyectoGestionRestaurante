using Aplicaciones.DTOs;
using Modelos;

namespace Aplicaciones.Interfaces
{
    public interface IGastoServicio
    {
        Resultado Registrar(GastoEntrada entrada, int empleadoId);
        Resultado Actualizar(int id, GastoEntrada entrada);
        Resultado ListarPorFecha(DateOnly fecha);
    }
}
