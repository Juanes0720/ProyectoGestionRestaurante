namespace Aplicaciones.DTOs
{
    public class AperturaCajaSalida
    {
        public int Id { get; set; }
        public string Cajero { get; set; } = string.Empty;
        public decimal BaseInicial { get; set; }
        public DateTime Fecha { get; set; }
        public bool Abierta { get; set; }
        public string Aviso { get; set; } = string.Empty;
    }
}
