using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Modelos;

namespace Aplicaciones.Servicios
{
    public class CorteCajaServicio : ICorteCajaServicio
    {
        private readonly IAperturaCajaRepositorio _aperturas;
        private readonly IRegistroHorasRepositorio _horas;
        private readonly ICorteRepositorio _cortes;

        public CorteCajaServicio(IAperturaCajaRepositorio aperturas, IRegistroHorasRepositorio horas, ICorteRepositorio cortes)
        {
            _aperturas = aperturas;
            _horas = horas;
            _cortes = cortes;
        }

        public Resultado HacerCorte(bool cerrarCaja)
        {
            // Solo con caja abierta.
            var apertura = _aperturas.ObtenerAbierta();
            if (apertura == null)
                return Resultado.Error("No hay una caja abierta para hacer el corte.");

            if (cerrarCaja)
            {
                var turnosAbiertos = _horas.ObtenerAbiertos();
                if (turnosAbiertos.Count > 0)
                {
                    var nombres = string.Join(", ", turnosAbiertos.Select(t => t.Empleado.Nombre));
                    return Resultado.Error($"Hay turnos abiertos: {nombres}. Cierre primero los turnos (POST api/horas/salida) para cerrar la caja.");
                }
                apertura.Cerrar();
            }

            var horaCorte = DateTime.Now;
            var salida = new CorteSalida
            {
                AperturaId = apertura.Id,
                Fecha = FormatoFecha.SoloFecha(horaCorte),
                HoraInicioTurno = FormatoFecha.SoloHora(apertura.Fecha),
                HoraCorte = FormatoFecha.SoloHora(horaCorte),
                CajaCerrada = cerrarCaja,
                BaseInicial = apertura.BaseInicial,
                TotalGastos = apertura.TotalGastos,
                TotalVentas = apertura.TotalVentas,
                TotalEsperado = apertura.Saldo
            };
            if (cerrarCaja)
                _cortes.Agregar(salida);

            return Resultado.Ok(salida, cerrarCaja ? "Corte realizado. La caja quedó cerrada." : "Corte parcial realizado. La caja continúa abierta.");
        }

        public Resultado Listar()
        {
            return Resultado.Ok(_cortes.ObtenerTodos().Where(c => c.CajaCerrada).ToList());
        }
    }
}
