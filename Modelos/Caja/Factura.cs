using Modelos.Empleados;
using Modelos.Ventas;

namespace Modelos.Caja
{
    public class Factura
    {
        public int Numero { get; set; }      // consecutivo, lo asigna el repositorio
        public DateTime Fecha { get; }
        public Cajero Cajero { get; }
        public Venta Venta { get; }
        public decimal Total { get; }
        public decimal ValorPagado { get; }
        public decimal Cambio { get; }

        public Factura(Cajero cajero, Venta venta, decimal valorPagado)
        {
            Fecha = DateTime.Now;
            Cajero = cajero;
            Venta = venta;
            Total = venta.Total;
            ValorPagado = valorPagado;
            Cambio = valorPagado - venta.Total;
        }
    }
}
