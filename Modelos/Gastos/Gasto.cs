using Modelos.Empleados;

namespace Modelos.Gastos
{
    public class Gasto
    {
        public int Id { get; set; }
        public string Descripcion { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime Fecha { get; }
        public Empleado Empleado { get; }       // quien lo registró
        public Modelos.Caja.AperturaCaja Apertura { get; }

        public Gasto(string descripcion, decimal valor, Empleado empleado, Modelos.Caja.AperturaCaja apertura)
        {
            Descripcion = descripcion;
            Valor = valor;
            Empleado = empleado;
            Apertura = apertura;
            Fecha = DateTime.Now;
        }

        public bool Actualizar(string descripcion, decimal valor)
        {
            if (!Apertura.ActualizarGasto(Id, valor)) return false;
            Descripcion = descripcion;
            Valor = valor;
            return true;
        }
    }
}
