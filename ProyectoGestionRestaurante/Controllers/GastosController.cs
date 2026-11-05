using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGestionRestaurante.Seguridad;

namespace ProyectoGestionRestaurante.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Todos)]
    [Route("api/gastos")]
    public class GastosController : ControladorBase
    {
        private readonly IGastoServicio _servicio;

        public GastosController(IGastoServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpPost]
        public IActionResult Registrar([FromBody] GastoEntrada entrada)
        {
            return Responder(_servicio.Registrar(entrada, EmpleadoActualId));
        }

        [Authorize(Roles = Roles.Supervisor)]
        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] GastoEntrada entrada)
        {
            return Responder(_servicio.Actualizar(id, entrada));
        }

        // Ejemplo: GET /api/gastos?fecha=2026-11-03
        [Authorize(Roles = Roles.Supervisor)]   // solo el supervisor consulta los gastos
        [HttpGet]
        public IActionResult ListarPorFecha([FromQuery] DateOnly fecha)
        {
            return Responder(_servicio.ListarPorFecha(fecha));
        }
    }
}
