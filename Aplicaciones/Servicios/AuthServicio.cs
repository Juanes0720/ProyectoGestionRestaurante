using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Modelos;

namespace Aplicaciones.Servicios
{
    public class AuthServicio : IAuthServicio
    {
        private readonly IEmpleadoRepositorio _empleados;
        private readonly IGeneradorToken _generadorToken;

        public AuthServicio(IEmpleadoRepositorio empleados, IGeneradorToken generadorToken)
        {
            _empleados = empleados;
            _generadorToken = generadorToken;
        }

        public Resultado Login(LoginEntrada entrada)
        {
            var empleado = _empleados.ObtenerPorNombre((entrada.Nombre ?? "").Trim());

            // El mismo mensaje para "no existe" y "contraseña mala": no se revela cuál falló.
            if (empleado == null || !Contrasenas.Verificar(entrada.Contrasena ?? "", empleado.ContrasenaHash))
                return Resultado.Error("Nombre o contraseña incorrectos.");

            return Resultado.Ok(new LoginSalida
            {
                Id = empleado.Id,
                Token = _generadorToken.Generar(empleado),
                Nombre = empleado.Nombre,
                Tipo = empleado.Tipo
            }, "Sesión iniciada.");
        }
    }
}
