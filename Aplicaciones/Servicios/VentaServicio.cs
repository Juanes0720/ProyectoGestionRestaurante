using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Modelos;
using Modelos.Ventas;

namespace Aplicaciones.Servicios
{
    public class VentaServicio : IVentaServicio
    {
        private readonly IVentaRepositorio _ventas;

        public VentaServicio(IVentaRepositorio ventas)
        {
            _ventas = ventas;
        }

        public Resultado Consultar(int id)
        {
            var venta = _ventas.ObtenerPorId(id);
            if (venta == null)
                return Resultado.Error("La venta no existe.");

            return Resultado.Ok(Mapear(venta));
        }

        private static VentaSalida Mapear(Venta v)
        {
            return new VentaSalida
            {
                Id = v.Id,
                Detalles = v.Detalles.Select(DetalleVentaMapper.Mapear).ToList(),
                Subtotal = v.Subtotal,
                Descuento = v.Descuento,
                Total = v.Total,
                Cobrada = v.Cobrada
            };
        }
    }
}
