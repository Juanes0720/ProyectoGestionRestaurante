using Aplicaciones.DTOs;
using Modelos.Caja;
using Modelos.Pedidos;

namespace Aplicaciones.Servicios
{
    // Convierte una Factura en su DTO de salida (lo usan CajaRegistradora y CajaServicio).
    public static class FacturaMapper
    {
        public static FacturaSalida Mapear(Factura f, Pedido? pedido = null)
        {
            return new FacturaSalida
            {
                Numero = f.Numero,
                Fecha = f.Fecha,
                Cajero = f.Cajero.Nombre,
                VentaId = f.Venta.Id,
                PedidoId = pedido?.Id,
                FechaPedido = pedido == null ? null : FormatoFecha.SoloFecha(pedido.Fecha),
                HoraPedido = pedido == null ? null : FormatoFecha.SoloHora(pedido.Fecha),
                TipoPedido = pedido?.Tipo ?? string.Empty,
                Detalles = f.Venta.Detalles.Select(DetalleVentaMapper.Mapear).ToList(),
                Subtotal = f.Venta.Subtotal,
                Descuento = f.Venta.Descuento,
                Total = f.Total,
                ValorPagado = f.ValorPagado,
                Cambio = f.Cambio
            };
        }
    }
}
