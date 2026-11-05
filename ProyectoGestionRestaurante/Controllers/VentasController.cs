using Aplicaciones.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGestionRestaurante.Seguridad;

namespace ProyectoGestionRestaurante.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Todos)]
    [Route("api/ventas")]
    public class VentasController : ControladorBase
    {
        private readonly IVentaServicio _servicio;

        public VentasController(IVentaServicio servicio)
        {
            _servicio = servicio;
        }

        // Las ventas se crean desde el pedido; aquí solo se consultan.
        [HttpGet("{id}")]
        public IActionResult Consultar(int id)
        {
            return Responder(_servicio.Consultar(id));
        }
    }
}
