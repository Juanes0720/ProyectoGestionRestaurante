namespace Modelos.Ventas
{
    public class Venta
    {
        private readonly List<DetalleVenta> _detalles = new();

        public int Id { get; set; }

        // Solo lectura desde afuera: para agregar se usa AgregarDetalle (que respeta las reglas).
        public IReadOnlyList<DetalleVenta> Detalles => _detalles;

        // Porcentaje de descuento (0 a 100). Reservado para cuando se implementen los descuentos.
        public decimal Descuento { get; private set; }

        public bool Cobrada { get; private set; }
        public int? FacturaNumero { get; private set; }

        // La venta calcula sus propios totales.
        public decimal Subtotal => _detalles.Sum(d => d.Subtotal);
        public decimal Total => Math.Round(Subtotal - (Subtotal * Descuento / 100), 2);

        // Regla: una venta ya cobrada no se modifica.
        public bool AgregarDetalle(DetalleVenta detalle)
        {
            if (Cobrada) return false;
            _detalles.Add(detalle);
            return true;
        }

        public bool AplicarDescuento(decimal porcentaje)
        {
            if (Cobrada) return false;
            Descuento = porcentaje;
            return true;
        }

        public bool ReemplazarDetalles(IEnumerable<DetalleVenta> detalles)
        {
            if (Cobrada) return false;
            _detalles.Clear();
            _detalles.AddRange(detalles);
            return true;
        }

        public void AsignarFactura(int numero)
        {
            FacturaNumero = numero;
        }

        public void MarcarCobrada()
        {
            Cobrada = true;
        }
    }
}
