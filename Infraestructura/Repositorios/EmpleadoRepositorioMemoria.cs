using Aplicaciones.Interfaces;
using Modelos.Empleados;

namespace Infraestructura.Repositorios
{
    public class EmpleadoRepositorioMemoria : IEmpleadoRepositorio
    {
        private readonly List<Empleado> _empleados = new();
        private int _siguienteId = 1;

        public void Agregar(Empleado empleado)
        {
            empleado.Id = _siguienteId++;
            _empleados.Add(empleado);
        }

        public Empleado? ObtenerPorId(int id)
        {
            return _empleados.FirstOrDefault(e => e.Id == id);
        }

        public Empleado? ObtenerPorNombre(string nombre)
        {
            return _empleados.FirstOrDefault(e =>
                string.Equals(e.Nombre, nombre, StringComparison.OrdinalIgnoreCase));
        }

        public List<Empleado> ObtenerTodos()
        {
            return _empleados.ToList();
        }
    }
}
