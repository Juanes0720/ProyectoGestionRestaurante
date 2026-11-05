using Modelos.Empleados;

namespace Aplicaciones.Interfaces
{
    public interface IEmpleadoRepositorio
    {
        void Agregar(Empleado empleado);          // asigna el Id
        Empleado? ObtenerPorId(int id);
        Empleado? ObtenerPorNombre(string nombre);
        List<Empleado> ObtenerTodos();
    }
}
