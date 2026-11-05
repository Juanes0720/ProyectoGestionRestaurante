using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGestionRestaurante.Seguridad;

namespace ProyectoGestionRestaurante.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Todos)]
    [Route("api/caja")]
    public class CajaController : ControladorBase
    {
        private readonly ICajaServicio _servicio;

        public CajaController(ICajaServicio servicio)
        {
            _servicio = servicio;
        }

        [Authorize(Roles = Roles.Cajero)]   // solo un cajero abre la caja
        [HttpPost("abrir")]
        public IActionResult Abrir([FromBody] AbrirCajaEntrada entrada)
        {
            return Responder(_servicio.Abrir(entrada, EmpleadoActualId));
        }

        [Authorize(Roles = Roles.Cajero)]   // solo un cajero cobra
        [HttpPost("cobrar")]
        public IActionResult Cobrar([FromBody] CobrarEntrada entrada)
        {
            return Responder(_servicio.Cobrar(entrada, EmpleadoActualId));
        }

        [HttpGet("saldo")]
        public IActionResult ConsultarSaldo()
        {
            return Responder(_servicio.ConsultarSaldo());
        }

        [HttpGet("factura/{numero}")]
        public IActionResult ConsultarFactura(int numero)
        {
            return Responder(_servicio.ConsultarFactura(numero));
        }
    }
}
