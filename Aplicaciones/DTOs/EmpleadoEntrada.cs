namespace Aplicaciones.DTOs
{
    public class EmpleadoEntrada
    {
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;   // "Cajero" o "Supervisor"
        public string Contrasena { get; set; } = string.Empty;
    }
}
