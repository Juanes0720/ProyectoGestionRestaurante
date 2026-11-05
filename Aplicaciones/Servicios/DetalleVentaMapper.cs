using Aplicaciones.DTOs;
using Modelos.Ventas;

namespace Aplicaciones.Servicios
{
    // Convierte un DetalleVenta en su DTO de salida (lo usan VentaServicio y PedidoServicio).
    public static class DetalleVentaMapper
    {
        public static DetalleVentaSalida Mapear(DetalleVenta d)
        {
            return new DetalleVentaSalida
            {
                ProductoId = d.ProductoId,
                Descripcion = d.Descripcion,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal
            };
        }
    }
}
