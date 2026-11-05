using Modelos.Empleados;

namespace Modelos.Caja
{
    public class AperturaCaja
    {
        private readonly List<MovimientoCaja> _movimientos = new();

        public int Id { get; set; }
        public Cajero Cajero { get; }
        public decimal BaseInicial { get; }
        public DateTime Fecha { get; }                       // hora de inicio del turno
        public DateTime? FechaCierre { get; private set; }   // hora del corte (null mientras esté abierta)
        public bool Abierta { get; private set; } = true;

        // Solo lectura desde afuera: los movimientos se agregan con AgregarMovimiento.
        public IReadOnlyList<MovimientoCaja> Movimientos => _movimientos;

        public AperturaCaja(Cajero cajero, decimal baseInicial)
        {
            Cajero = cajero;
            BaseInicial = baseInicial;
            Fecha = DateTime.Now;
        }

        public void AgregarMovimiento(string tipo, decimal valor, int? referenciaId = null)
        {
            _movimientos.Add(new MovimientoCaja(tipo, valor, referenciaId));
        }

        public bool ActualizarGasto(int gastoId, decimal valor)
        {
            var movimiento = _movimientos.FirstOrDefault(m => m.Tipo == TipoMovimiento.Gasto && m.ReferenciaId == gastoId);
            if (movimiento == null) return false;
            movimiento.ActualizarValor(valor);
            return true;
        }

        public void Cerrar()
        {
            Abierta = false;
            FechaCierre = DateTime.Now;
        }

        // Se calculan desde los movimientos de ESTA apertura.
        public decimal TotalVentas =>
            _movimientos.Where(m => m.Tipo == TipoMovimiento.Venta).Sum(m => m.Valor);

        public decimal TotalGastos =>
            _movimientos.Where(m => m.Tipo == TipoMovimiento.Gasto).Sum(m => m.Valor);

        // Saldo = base inicial + ventas - gastos
        public decimal Saldo => BaseInicial + TotalVentas - TotalGastos;
    }
}
