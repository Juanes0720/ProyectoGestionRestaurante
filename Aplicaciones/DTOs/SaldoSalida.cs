namespace Aplicaciones.DTOs
{
    public class SaldoSalida
    {
        public int AperturaId { get; set; }
        public decimal BaseInicial { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal Saldo { get; set; }
    }
}
