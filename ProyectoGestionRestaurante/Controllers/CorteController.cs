using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGestionRestaurante.Seguridad;

namespace ProyectoGestionRestaurante.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Todos)]
    [Route("api/corte")]
    public class CorteController : ControladorBase
    {
        private readonly ICorteCajaServicio _servicio;

        public CorteController(ICorteCajaServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpPost]
        public IActionResult HacerCorte([FromQuery] bool cerrarCaja = true)
        {
            return Responder(_servicio.HacerCorte(cerrarCaja));
        }

        [HttpGet]
        public IActionResult Listar()
        {
            return Responder(_servicio.Listar());
        }
    }
}
