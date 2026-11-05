using Aplicaciones.DTOs;
using Modelos;

namespace Aplicaciones.Interfaces
{
    public interface IEmpleadoServicio
    {
        Resultado Crear(EmpleadoEntrada entrada);
        Resultado Listar();
    }
}
