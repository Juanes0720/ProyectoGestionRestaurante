namespace Modelos.Ventas
{
    // Todo se COPIA al momento de la venta (id, nombre y precio):
    // si luego el producto cambia de nombre o de precio, la venta no se altera.
    public class DetalleVenta
    {
        public int? ProductoId { get; }          // null para conceptos que no son productos (ej. envío)
        public string Descripcion { get; }
        public int Cantidad { get; }
        public decimal PrecioUnitario { get; }

        public decimal Subtotal => Cantidad * PrecioUnitario;

        // Detalle de un producto del menú
        public DetalleVenta(Producto producto, int cantidad)
            : this(producto.Id, producto.Nombre, cantidad, producto.Precio)
        {
        }

        // Detalle genérico (por ejemplo, el envío a domicilio)
        public DetalleVenta(int? productoId, string descripcion, int cantidad, decimal precioUnitario)
        {
            ProductoId = productoId;
            Descripcion = descripcion;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
        }
    }
}
