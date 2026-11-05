using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Modelos;
using Modelos.Caja;
using Modelos.Gastos;

namespace Aplicaciones.Servicios
{
    public class GastoServicio : IGastoServicio
    {
        private readonly IGastoRepositorio _gastos;
        private readonly IEmpleadoRepositorio _empleados;
        private readonly IAperturaCajaRepositorio _aperturas;

        public GastoServicio(IGastoRepositorio gastos, IEmpleadoRepositorio empleados,
                             IAperturaCajaRepositorio aperturas)
        {
            _gastos = gastos;
            _empleados = empleados;
            _aperturas = aperturas;
        }

        public Resultado Registrar(GastoEntrada entrada, int empleadoId)
        {
            if (string.IsNullOrWhiteSpace(entrada.Descripcion))
                return Resultado.Error("La descripción es obligatoria.");

            if (entrada.Valor <= 0)
                return Resultado.Error("El valor debe ser mayor que cero.");

            var empleado = _empleados.ObtenerPorId(empleadoId);
            if (empleado == null)
                return Resultado.Error("El empleado no existe.");

            var apertura = _aperturas.ObtenerAbierta();
            if (apertura == null)
                return Resultado.Error("La caja está cerrada. Abra una caja antes de registrar gastos.");

            var gasto = new Gasto(entrada.Descripcion.Trim(), entrada.Valor, empleado, apertura);
            _gastos.Agregar(gasto);

            // El gasto se agrega como movimiento a la apertura actual (el saldo baja).
            apertura.AgregarMovimiento(TipoMovimiento.Gasto, gasto.Valor, gasto.Id);

            return Resultado.Ok(Mapear(gasto), "Gasto registrado.");
        }

        public Resultado Actualizar(int id, GastoEntrada entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada.Descripcion))
                return Resultado.Error("La descripción es obligatoria.");
            if (entrada.Valor <= 0)
                return Resultado.Error("El valor debe ser mayor que cero.");

            var gasto = _gastos.ObtenerPorId(id);
            if (gasto == null)
                return Resultado.Error("El gasto no existe.");
            if (!gasto.Actualizar(entrada.Descripcion.Trim(), entrada.Valor))
                return Resultado.Error("No se pudo actualizar el movimiento de caja del gasto.");

            return Resultado.Ok(Mapear(gasto), "Gasto actualizado.");
        }

        public Resultado ListarPorFecha(DateOnly fecha)
        {
            var lista = _gastos.ObtenerPorFecha(fecha).Select(Mapear).ToList();
            return Resultado.Ok(lista);
        }

        private static GastoSalida Mapear(Gasto g)
        {
            return new GastoSalida
            {
                Id = g.Id,
                Descripcion = g.Descripcion,
                Valor = g.Valor,
                Fecha = FormatoFecha.SoloFecha(g.Fecha),
                Hora = FormatoFecha.SoloHora(g.Fecha),
                Empleado = g.Empleado.Nombre
            };
        }
    }
}
