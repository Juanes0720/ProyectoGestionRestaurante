using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Modelos;
using Modelos.Empleados;

namespace Aplicaciones.Servicios
{
    public class EmpleadoServicio : IEmpleadoServicio
    {
        private readonly IEmpleadoRepositorio _repositorio;

        public EmpleadoServicio(IEmpleadoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public Resultado Crear(EmpleadoEntrada entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada.Nombre))
                return Resultado.Error("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(entrada.Contrasena) || entrada.Contrasena.Length < 6)
                return Resultado.Error("La contraseña es obligatoria y debe tener mínimo 6 caracteres.");

            // El nombre se usa para iniciar sesión, así que no puede repetirse.
            if (_repositorio.ObtenerPorNombre(entrada.Nombre.Trim()) != null)
                return Resultado.Error("Ya existe un empleado con ese nombre.");

            var tipo = (entrada.Tipo ?? "").Trim().ToLower();

            Empleado empleado;
            if (tipo == "cajero")
                empleado = new Cajero { Nombre = entrada.Nombre.Trim() };
            else if (tipo == "supervisor")
                empleado = new Supervisor { Nombre = entrada.Nombre.Trim() };
            else
                return Resultado.Error("Tipo no válido. Use 'Cajero' o 'Supervisor'.");

            empleado.ContrasenaHash = Contrasenas.Hashear(entrada.Contrasena);
            _repositorio.Agregar(empleado);
            return Resultado.Ok(Mapear(empleado), "Empleado creado.");
        }

        public Resultado Listar()
        {
            var lista = _repositorio.ObtenerTodos().Select(Mapear).ToList();
            return Resultado.Ok(lista);
        }

        private static EmpleadoSalida Mapear(Empleado e)
        {
            return new EmpleadoSalida { Id = e.Id, Nombre = e.Nombre, Tipo = e.Tipo };
        }
    }
}
