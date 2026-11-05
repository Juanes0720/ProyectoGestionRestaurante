using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGestionRestaurante.Seguridad;

namespace ProyectoGestionRestaurante.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Todos)]
    [Route("api/horas")]
    public class HorasController : ControladorBase
    {
        private readonly IRegistroHorasServicio _servicio;

        public HorasController(IRegistroHorasServicio servicio)
        {
            _servicio = servicio;
        }

        // El cajero solo maneja sus propios turnos; el supervisor, los de cualquiera.
        private bool PuedeActuarSobre(int empleadoId)
        {
            return EsSupervisor || empleadoId == EmpleadoActualId;
        }

        [HttpPost("entrada")]
        public IActionResult RegistrarEntrada([FromBody] RegistroHorasEntrada entrada)
        {
            if (!PuedeActuarSobre(entrada.EmpleadoId)) return Forbid();
            return Responder(_servicio.RegistrarEntrada(entrada));
        }

        [HttpPost("salida")]
        public IActionResult RegistrarSalida([FromBody] RegistroHorasEntrada entrada)
        {
            if (!PuedeActuarSobre(entrada.EmpleadoId)) return Forbid();
            return Responder(_servicio.RegistrarSalida(entrada));
        }

        [HttpGet("{empleadoId}")]
        public IActionResult ConsultarHoras(int empleadoId)
        {
            if (!PuedeActuarSobre(empleadoId)) return Forbid();
            return Responder(_servicio.ConsultarHoras(empleadoId));
        }
    }
}
