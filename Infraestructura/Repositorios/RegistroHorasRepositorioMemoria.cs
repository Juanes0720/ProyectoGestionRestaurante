using Aplicaciones.Interfaces;
using Modelos.Empleados;

namespace Infraestructura.Repositorios
{
    public class RegistroHorasRepositorioMemoria : IRegistroHorasRepositorio
    {
        private readonly List<RegistroHoras> _registros = new();
        private int _siguienteId = 1;

        public void Agregar(RegistroHoras registro)
        {
            registro.Id = _siguienteId++;
            _registros.Add(registro);
        }

        public RegistroHoras? ObtenerAbierto(int empleadoId)
        {
            return _registros.FirstOrDefault(r => r.Empleado.Id == empleadoId && r.EstaAbierto);
        }

        public List<RegistroHoras> ObtenerAbiertos()
        {
            return _registros.Where(r => r.EstaAbierto).ToList();
        }

        public List<RegistroHoras> ObtenerPorEmpleado(int empleadoId)
        {
            return _registros.Where(r => r.Empleado.Id == empleadoId).ToList();
        }
    }
}
