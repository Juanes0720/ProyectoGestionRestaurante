namespace Aplicaciones.DTOs
{
    // El empleado que registra el gasto es quien inició sesión (sale del token).
    public class GastoEntrada
    {
        public string Descripcion { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}
