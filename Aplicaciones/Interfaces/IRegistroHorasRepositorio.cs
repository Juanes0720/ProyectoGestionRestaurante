using Modelos.Empleados;

namespace Aplicaciones.Interfaces
{
    public interface IRegistroHorasRepositorio
    {
        void Agregar(RegistroHoras registro);     // asigna el Id
        RegistroHoras? ObtenerAbierto(int empleadoId);
        List<RegistroHoras> ObtenerPorEmpleado(int empleadoId);
        List<RegistroHoras> ObtenerAbiertos();    // todos los turnos sin salida
    }
}
