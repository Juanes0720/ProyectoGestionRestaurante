namespace Modelos.Caja
{
    public class MovimientoCaja
    {
        public string Tipo { get; }          // "venta" o "gasto"
        public decimal Valor { get; private set; }
        public DateTime Fecha { get; }
        public int? ReferenciaId { get; }

        public MovimientoCaja(string tipo, decimal valor, int? referenciaId = null)
        {
            Tipo = tipo;
            Valor = valor;
            ReferenciaId = referenciaId;
            Fecha = DateTime.Now;
        }

        public void ActualizarValor(decimal valor)
        {
            Valor = valor;
        }
    }
}
