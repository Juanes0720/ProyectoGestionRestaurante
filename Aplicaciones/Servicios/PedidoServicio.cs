using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Modelos;
using Modelos.Pedidos;
using Modelos.Ventas;

namespace Aplicaciones.Servicios
{
    public class PedidoServicio : IPedidoServicio
    {
        private readonly IProductoRepositorio _productos;
        private readonly IPedidoRepositorio _pedidos;
        private readonly IVentaRepositorio _ventas;
        private readonly IAperturaCajaRepositorio _aperturas;

        public PedidoServicio(IProductoRepositorio productos, IPedidoRepositorio pedidos,
                              IVentaRepositorio ventas, IAperturaCajaRepositorio aperturas)
        {
            _productos = productos;
            _pedidos = pedidos;
            _ventas = ventas;
            _aperturas = aperturas;
        }

        public Resultado Crear(PedidoEntrada entrada)
        {
            var apertura = _aperturas.ObtenerAbierta();
            if (apertura == null)
                return Resultado.Error("Abra la caja antes de crear pedidos.");

            if (entrada.Items == null || entrada.Items.Count == 0)
                return Resultado.Error("El pedido debe tener al menos un producto.");

            // 1) Crear el pedido según el tipo, validando los datos obligatorios de ese tipo.
            var tipo = (entrada.Tipo ?? "").Trim().ToLower();
            Pedido pedido;

            if (tipo == "comerlocal")
            {
                if (entrada.Mesa == null || entrada.Mesa <= 0)
                    return Resultado.Error("La mesa es obligatoria y debe ser mayor que cero.");

                pedido = new PedidoComerLocal { Mesa = entrada.Mesa.Value };
            }
            else if (tipo == "domicilio")
            {
                if (string.IsNullOrWhiteSpace(entrada.Direccion))
                    return Resultado.Error("La dirección es obligatoria.");
                if (string.IsNullOrWhiteSpace(entrada.Telefono))
                    return Resultado.Error("El teléfono es obligatorio.");
                if (entrada.CostoEnvio < 0)
                    return Resultado.Error("El costo de envío no puede ser negativo.");

                pedido = new PedidoDomicilio
                {
                    Direccion = entrada.Direccion.Trim(),
                    Telefono = entrada.Telefono.Trim(),
                    CostoEnvio = entrada.CostoEnvio
                };
            }
            else if (tipo == "recogertienda")
            {
                if (string.IsNullOrWhiteSpace(entrada.NombreCliente))
                    return Resultado.Error("El nombre del cliente es obligatorio.");
                pedido = new PedidoRecogerTienda
                {
                    NombreCliente = entrada.NombreCliente.Trim()
                };
            }
            else
            {
                return Resultado.Error("Tipo no válido. Use 'ComerLocal', 'Domicilio' o 'RecogerTienda'.");
            }

            // 2) Agregar los productos al pedido.
            foreach (var item in entrada.Items)
            {
                if (item.Cantidad <= 0)
                    return Resultado.Error("La cantidad debe ser mayor que cero.");

                var producto = _productos.ObtenerPorId(item.ProductoId);
                if (producto == null)
                    return Resultado.Error($"El producto {item.ProductoId} no existe.");

                pedido.AgregarDetalle(new DetalleVenta(producto, item.Cantidad));
            }

            // 3) Crear la venta del pedido (ya pasaron todas las validaciones).
            var venta = new Venta();
            foreach (var detalle in pedido.Detalles)
                venta.AgregarDetalle(detalle);

            // El envío entra a la venta como un detalle más.
            if (pedido is PedidoDomicilio domicilio && domicilio.CostoEnvio > 0)
                venta.AgregarDetalle(new DetalleVenta(null, "Envío a domicilio", 1, domicilio.CostoEnvio));

            _ventas.Agregar(venta);

            // 4) El pedido queda asociado a la venta y a la caja de este turno.
            pedido.AsignarVenta(venta.Id);
            pedido.AsignarAperturaCaja(apertura.Id);
            _pedidos.Agregar(pedido);

            return Resultado.Ok(Mapear(pedido), "Pedido creado.");
        }

        public Resultado Actualizar(int id, PedidoEntrada entrada)
        {
            var pedido = _pedidos.ObtenerPorId(id);
            if (pedido == null)
                return Resultado.Error("El pedido no existe.");

            var venta = _ventas.ObtenerPorId(pedido.VentaId);
            if (venta == null)
                return Resultado.Error("La venta del pedido no existe.");
            if (venta.Cobrada)
                return Resultado.Error("No se puede modificar un pedido que ya fue cobrado.");
            if (!string.Equals(pedido.Tipo, entrada.Tipo, StringComparison.OrdinalIgnoreCase))
                return Resultado.Error("No se puede cambiar el tipo de un pedido existente.");
            if (entrada.Items == null || entrada.Items.Count == 0)
                return Resultado.Error("El pedido debe tener al menos un producto.");

            if (pedido is PedidoComerLocal && (entrada.Mesa == null || entrada.Mesa <= 0))
                return Resultado.Error("La mesa es obligatoria y debe ser mayor que cero.");
            if (pedido is PedidoDomicilio && (string.IsNullOrWhiteSpace(entrada.Direccion) || string.IsNullOrWhiteSpace(entrada.Telefono) || entrada.CostoEnvio < 0))
                return Resultado.Error("La dirección y el teléfono son obligatorios; el costo de envío no puede ser negativo.");
            if (pedido is PedidoRecogerTienda && string.IsNullOrWhiteSpace(entrada.NombreCliente))
                return Resultado.Error("El nombre del cliente es obligatorio.");

            var detalles = new List<DetalleVenta>();
            foreach (var item in entrada.Items)
            {
                if (item.Cantidad <= 0)
                    return Resultado.Error("La cantidad debe ser mayor que cero.");
                var producto = _productos.ObtenerPorId(item.ProductoId);
                if (producto == null)
                    return Resultado.Error($"El producto {item.ProductoId} no existe.");
                detalles.Add(new DetalleVenta(producto, item.Cantidad));
            }

            pedido.ReemplazarDetalles(detalles);
            venta.ReemplazarDetalles(detalles);
            if (pedido is PedidoComerLocal local)
                local.Mesa = entrada.Mesa!.Value;
            else if (pedido is PedidoDomicilio domicilio)
            {
                domicilio.Direccion = entrada.Direccion!.Trim();
                domicilio.Telefono = entrada.Telefono!.Trim();
                domicilio.CostoEnvio = entrada.CostoEnvio;
                if (domicilio.CostoEnvio > 0)
                    venta.AgregarDetalle(new DetalleVenta(null, "Envío a domicilio", 1, domicilio.CostoEnvio));
            }
            else if (pedido is PedidoRecogerTienda recoger)
                recoger.NombreCliente = entrada.NombreCliente!.Trim();

            return Resultado.Ok(Mapear(pedido), "Pedido actualizado.");
        }

        public Resultado Consultar(int id)
        {
            var pedido = _pedidos.ObtenerPorId(id);
            if (pedido == null)
                return Resultado.Error("El pedido no existe.");

            return Resultado.Ok(Mapear(pedido));
        }

        public Resultado Listar()
        {
            var apertura = _aperturas.ObtenerAbierta();
            var lista = apertura == null
                ? new List<PedidoSalida>()
                : _pedidos.ObtenerTodos()
                    .Where(p => p.AperturaCajaId == apertura.Id)
                    .Select(Mapear)
                    .ToList();
            return Resultado.Ok(lista);
        }

        private PedidoSalida Mapear(Pedido p)
        {
            var salida = new PedidoSalida
            {
                Id = p.Id,
                Fecha = FormatoFecha.SoloFecha(p.Fecha),
                HoraPedido = FormatoFecha.SoloHora(p.Fecha),
                Tipo = p.Tipo,
                Detalles = p.Detalles.Select(DetalleVentaMapper.Mapear).ToList(),
                Total = p.Total,
                VentaId = p.VentaId,
                Cobrado = _ventas.ObtenerPorId(p.VentaId)?.Cobrada ?? false,
                FacturaNumero = _ventas.ObtenerPorId(p.VentaId)?.FacturaNumero
            };

            if (p is PedidoComerLocal local)
            {
                salida.Mesa = local.Mesa;
            }
            else if (p is PedidoDomicilio domicilio)
            {
                salida.Direccion = domicilio.Direccion;
                salida.Telefono = domicilio.Telefono;
                salida.CostoEnvio = domicilio.CostoEnvio;
            }
            else if (p is PedidoRecogerTienda recoger)
            {
                salida.NombreCliente = recoger.NombreCliente;
            }

            return salida;
        }
    }
}
