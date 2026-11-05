using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGestionRestaurante.Seguridad;

namespace ProyectoGestionRestaurante.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Todos)]
    [Route("api/productos")]
    public class ProductosController : ControladorBase
    {
        private readonly IProductoServicio _servicio;

        public ProductosController(IProductoServicio servicio)
        {
            _servicio = servicio;
        }

        [Authorize(Roles = Roles.Supervisor)]   // solo el supervisor crea productos
        [HttpPost]
        public IActionResult Crear([FromBody] ProductoEntrada entrada)
        {
            return Responder(_servicio.Crear(entrada));
        }

        [Authorize(Roles = Roles.Supervisor)]   // solo el supervisor edita productos
        [HttpPut("{id}")]
        public IActionResult Editar(int id, [FromBody] ProductoEntrada entrada)
        {
            return Responder(_servicio.Editar(id, entrada));
        }

        [HttpGet]
        public IActionResult Listar()
        {
            return Responder(_servicio.Listar());
        }
    }
}
