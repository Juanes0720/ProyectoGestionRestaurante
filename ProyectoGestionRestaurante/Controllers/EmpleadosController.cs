using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGestionRestaurante.Seguridad;

namespace ProyectoGestionRestaurante.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Supervisor)]   // solo el supervisor administra empleados
    [Route("api/empleados")]
    public class EmpleadosController : ControladorBase
    {
        private readonly IEmpleadoServicio _servicio;

        public EmpleadosController(IEmpleadoServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpPost]
        public IActionResult Crear([FromBody] EmpleadoEntrada entrada)
        {
            return Responder(_servicio.Crear(entrada));
        }

        [HttpGet]
        public IActionResult Listar()
        {
            return Responder(_servicio.Listar());
        }
    }
}
