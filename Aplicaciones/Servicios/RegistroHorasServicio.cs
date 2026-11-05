using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Modelos;
using Modelos.Empleados;

namespace Aplicaciones.Servicios
{
    public class RegistroHorasServicio : IRegistroHorasServicio
    {
        private readonly IEmpleadoRepositorio _empleados;
        private readonly IRegistroHorasRepositorio _registros;

        public RegistroHorasServicio(IEmpleadoRepositorio empleados, IRegistroHorasRepositorio registros)
        {
            _empleados = empleados;
            _registros = registros;
        }

        public Resultado RegistrarEntrada(RegistroHorasEntrada entrada)
        {
            var empleado = _empleados.ObtenerPorId(entrada.EmpleadoId);
            if (empleado == null)
                return Resultado.Error("El empleado no existe.");

            if (_registros.ObtenerAbierto(entrada.EmpleadoId) != null)
                return Resultado.Error("El empleado ya tiene una entrada abierta.");

            var ahora = DateTime.Now;
            var registro = new RegistroHoras(
                empleado,
                DateOnly.FromDateTime(ahora),
                FormatoFecha.SoloHora(ahora));

            _registros.Agregar(registro);
            return Resultado.Ok(new { registro.Id, Empleado = empleado.Nombre, registro.Fecha, registro.HoraEntrada },
                                "Entrada registrada.");
        }

        public Resultado RegistrarSalida(RegistroHorasEntrada entrada)
        {
            var empleado = _empleados.ObtenerPorId(entrada.EmpleadoId);
            if (empleado == null)
                return Resultado.Error("El empleado no existe.");

            var abierto = _registros.ObtenerAbierto(entrada.EmpleadoId);
            if (abierto == null)
                return Resultado.Error("El empleado no tiene una entrada abierta.");

            // Se guarda la hora de salida y las horas trabajadas en el registro.
            abierto.CerrarTurno(FormatoFecha.SoloHora(DateTime.Now));
            return Resultado.Ok(new
            {
                abierto.Id,
                Empleado = empleado.Nombre,
                abierto.HoraEntrada,
                abierto.HoraSalida,
                abierto.HorasTrabajadas
            }, "Salida registrada.");
        }

        public Resultado ConsultarHoras(int empleadoId)
        {
            var empleado = _empleados.ObtenerPorId(empleadoId);
            if (empleado == null)
                return Resultado.Error("El empleado no existe.");

            var registros = _registros.ObtenerPorEmpleado(empleadoId);
            var cerrados = registros.Where(r => !r.EstaAbierto).ToList();

            // Decisión: un registro abierto NO se cuenta en el total.
            var salida = new HorasSalida
            {
                EmpleadoId = empleado.Id,
                Nombre = empleado.Nombre,
                HorasTrabajadas = Math.Round(cerrados.Sum(r => r.HorasTrabajadas), 2),
                RegistrosCerrados = cerrados.Count,
                TieneEntradaAbierta = registros.Any(r => r.EstaAbierto),
                Registros = registros.Select(r => new RegistroHorasSalida
                {
                    Id = r.Id,
                    Fecha = r.Fecha,
                    HoraEntrada = r.HoraEntrada,
                    HoraSalida = r.HoraSalida,
                    HorasTrabajadas = r.HorasTrabajadas
                }).ToList()
            };
            return Resultado.Ok(salida);
        }
    }
}
