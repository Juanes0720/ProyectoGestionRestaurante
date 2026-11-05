using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Aplicaciones.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Modelos.Empleados;

namespace ProyectoGestionRestaurante.Seguridad
{
    // Fabrica el token JWT con la configuración de appsettings.json (sección JwtSettings).
    public class JwtService : IGeneradorToken
    {
        private readonly IConfiguration _configuracion;

        public JwtService(IConfiguration configuracion)
        {
            _configuracion = configuracion;
        }

        public string Generar(Empleado empleado)
        {
            var jwt = _configuracion.GetSection("JwtSettings");
            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            // Dentro del token viajan: quién es (id y nombre) y su rol (Cajero o Supervisor).
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, empleado.Id.ToString()),
                new Claim(ClaimTypes.Name, empleado.Nombre),
                new Claim(ClaimTypes.Role, empleado.Tipo)
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwt["DurationInMinutes"])),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
