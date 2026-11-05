using Aplicaciones.Interfaces;
using Modelos;
using Modelos.Caja;
using Modelos.Empleados;
using Modelos.Ventas;

namespace Aplicaciones.Servicios
{
    // Recibe la venta y el dinero: valida, calcula el cambio,
    // agrega el movimiento a la apertura y genera la factura.
    public class CajaRegistradora : ICajaRegistradora
    {
        private readonly IAperturaCajaRepositorio _aperturas;
        private readonly IFacturaRepositorio _facturas;
        private readonly IPedidoRepositorio _pedidos;

        public CajaRegistradora(IAperturaCajaRepositorio aperturas, IFacturaRepositorio facturas, IPedidoRepositorio pedidos)
        {
            _aperturas = aperturas;
            _facturas = facturas;
            _pedidos = pedidos;
        }

        public Resultado Cobrar(Venta venta, decimal dineroRecibido, Cajero cajero)
        {
            var apertura = _aperturas.ObtenerAbierta();
            if (apertura == null)
                return Resultado.Error("La caja está cerrada. Abra una caja antes de cobrar.");

            if (venta.Cobrada)
                return Resultado.Error("La venta ya fue cobrada.");

            if (dineroRecibido < venta.Total)
                return Resultado.Error($"El dinero recibido no alcanza. Total a pagar: {venta.Total}.");

            // Todo está bien: se cobra.
            var factura = new Factura(cajero, venta, dineroRecibido);

            venta.MarcarCobrada();
            apertura.AgregarMovimiento(TipoMovimiento.Venta, venta.Total);
            _facturas.Agregar(factura);
            venta.AsignarFactura(factura.Numero);
            var pedido = _pedidos.ObtenerTodos().FirstOrDefault(p => p.VentaId == venta.Id);

            return Resultado.Ok(FacturaMapper.Mapear(factura, pedido), "Venta cobrada.");
        }
    }
}
