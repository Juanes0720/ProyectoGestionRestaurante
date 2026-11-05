using Modelos.Ventas;

namespace Modelos.Pedidos
{
    public abstract class Pedido
    {
        private readonly List<DetalleVenta> _detalles = new();

        public int Id { get; set; }
        // Se asigna sola en el instante en que se crea el pedido.
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Los productos del pedido (solo lectura desde afuera).
        public IReadOnlyList<DetalleVenta> Detalles => _detalles;

        // Id de la venta del pedido. Se asigna al crear el pedido.
        public int VentaId { get; private set; }
        public int AperturaCajaId { get; private set; }

        // Cada subclase dice su tipo (texto).
        public abstract string Tipo { get; }

        // virtual: PedidoDomicilio lo cambia para sumar el envío.
        public virtual decimal Total => _detalles.Sum(d => d.Subtotal);

        public void AgregarDetalle(DetalleVenta detalle)
        {
            _detalles.Add(detalle);
        }

        public void ReemplazarDetalles(IEnumerable<DetalleVenta> detalles)
        {
            _detalles.Clear();
            _detalles.AddRange(detalles);
        }

        public void AsignarVenta(int ventaId)
        {
            VentaId = ventaId;
        }

        public void AsignarAperturaCaja(int aperturaCajaId)
        {
            AperturaCajaId = aperturaCajaId;
        }
    }
}
