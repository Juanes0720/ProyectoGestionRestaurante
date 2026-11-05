using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGestionRestaurante.Seguridad;

namespace ProyectoGestionRestaurante.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Todos)]
    [Route("api/pedidos")]
    public class PedidosController : ControladorBase
    {
        private readonly IPedidoServicio _servicio;

        public PedidosController(IPedidoServicio servicio)
        {
            _servicio = servicio;
        }

        // Crea el pedido y su venta. La respuesta trae el ventaId para cobrar.
        [HttpPost]
        public IActionResult Crear([FromBody] PedidoEntrada entrada)
        {
            return Responder(_servicio.Crear(entrada));
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] PedidoEntrada entrada)
        {
            return Responder(_servicio.Actualizar(id, entrada));
        }

        [HttpGet]
        public IActionResult Listar()
        {
            return Responder(_servicio.Listar());
        }

        [HttpGet("{id}")]
        public IActionResult Consultar(int id)
        {
            return Responder(_servicio.Consultar(id));
        }
    }
}
