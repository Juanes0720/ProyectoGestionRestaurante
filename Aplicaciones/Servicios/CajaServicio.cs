using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Modelos;
using Modelos.Caja;
using Modelos.Empleados;

namespace Aplicaciones.Servicios
{
    public class CajaServicio : ICajaServicio
    {
        private readonly IAperturaCajaRepositorio _aperturas;
        private readonly IFacturaRepositorio _facturas;
        private readonly IEmpleadoRepositorio _empleados;
        private readonly IVentaRepositorio _ventas;
        private readonly IPedidoRepositorio _pedidos;
        private readonly ICajaRegistradora _registradora;

        public CajaServicio(IAperturaCajaRepositorio aperturas, IFacturaRepositorio facturas,
                            IEmpleadoRepositorio empleados, IVentaRepositorio ventas,
                            IPedidoRepositorio pedidos, ICajaRegistradora registradora)
        {
            _aperturas = aperturas;
            _facturas = facturas;
            _empleados = empleados;
            _ventas = ventas;
            _pedidos = pedidos;
            _registradora = registradora;
        }

        public Resultado Abrir(AbrirCajaEntrada entrada, int cajeroId)
        {
            var (cajero, error) = ObtenerCajero(cajeroId, "abrir la caja");
            if (cajero == null)
                return Resultado.Error(error!);

            if (entrada.BaseInicial < 0)
                return Resultado.Error("La base inicial no puede ser negativa.");

            // Solo una caja abierta a la vez.
            if (_aperturas.ObtenerAbierta() != null)
                return Resultado.Error("Ya hay una caja abierta.");

            var apertura = new AperturaCaja(cajero, entrada.BaseInicial);
            _aperturas.Agregar(apertura);

            return Resultado.Ok(new AperturaCajaSalida
            {
                Id = apertura.Id,
                Cajero = cajero.Nombre,
                BaseInicial = apertura.BaseInicial,
                Fecha = apertura.Fecha,
                Abierta = apertura.Abierta,
                Aviso = "Se recomienda iniciar los turnos de los trabajadores."
            }, "Caja abierta.");
        }

        public Resultado Cobrar(CobrarEntrada entrada, int cajeroId)
        {
            var (cajero, error) = ObtenerCajero(cajeroId, "cobrar");
            if (cajero == null)
                return Resultado.Error(error!);

            var venta = _ventas.ObtenerPorId(entrada.VentaId);
            if (venta == null)
                return Resultado.Error("La venta no existe.");

            // El resto de las reglas (caja abierta, venta sin cobrar, dinero suficiente)
            // las valida la CajaRegistradora.
            return _registradora.Cobrar(venta, entrada.DineroRecibido, cajero);
        }

        public Resultado ConsultarSaldo()
        {
            var apertura = _aperturas.ObtenerAbierta();
            if (apertura == null)
                return Resultado.Error("No hay una caja abierta.");

            return Resultado.Ok(new SaldoSalida
            {
                AperturaId = apertura.Id,
                BaseInicial = apertura.BaseInicial,
                TotalVentas = apertura.TotalVentas,
                TotalGastos = apertura.TotalGastos,
                Saldo = apertura.Saldo
            });
        }

        public Resultado ConsultarFactura(int numero)
        {
            var factura = _facturas.ObtenerPorNumero(numero);
            if (factura == null)
                return Resultado.Error("La factura no existe.");

            var pedido = _pedidos.ObtenerTodos().FirstOrDefault(p => p.VentaId == factura.Venta.Id);
            return Resultado.Ok(FacturaMapper.Mapear(factura, pedido));
        }

        // Valida que el empleado exista
        // Devuelve el cajero, o null con el mensaje de error.
        private (Cajero? Cajero, string? Error) ObtenerCajero(int empleadoId, string accion)
        {
            var empleado = _empleados.ObtenerPorId(empleadoId);
            if (empleado == null)
                return (null, "El empleado no existe.");
            if (empleado is not Cajero cajero)
                return (null, $"Solo un Cajero puede {accion}.");

            return (cajero, null);
        }
    }
}
