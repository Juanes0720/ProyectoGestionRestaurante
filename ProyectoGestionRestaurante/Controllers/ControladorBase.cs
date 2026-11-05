using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Modelos;
using ProyectoGestionRestaurante.Seguridad;

namespace ProyectoGestionRestaurante.Controllers
{
    // 200 con los Datos si Exito; 400 con el Mensaje si no.
    public abstract class ControladorBase : ControllerBase
    {
        // Id del empleado que inició sesión (viene dentro del token).
        protected int EmpleadoActualId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        protected bool EsSupervisor => User.IsInRole(Roles.Supervisor);

        protected IActionResult Responder(Resultado resultado)
        {
            if (resultado.Exito)
                return Ok(resultado.Datos);

            return BadRequest(resultado.Mensaje);
        }
    }
}
