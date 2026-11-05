using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoGestionRestaurante.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControladorBase
    {
        private readonly IAuthServicio _servicio;

        public AuthController(IAuthServicio servicio)
        {
            _servicio = servicio;
        }

        // Único endpoint público: devuelve el token que se usa en todos los demás.
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginEntrada entrada)
        {
            var resultado = _servicio.Login(entrada);
            if (!resultado.Exito)
                return Unauthorized(resultado.Mensaje);

            return Ok(resultado.Datos);
        }
    }
}
