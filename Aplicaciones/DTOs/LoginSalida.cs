namespace Aplicaciones.DTOs
{
    public class LoginSalida
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }
}
