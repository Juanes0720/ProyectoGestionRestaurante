using Modelos;
using Modelos.Empleados;
using Modelos.Ventas;

namespace Aplicaciones.Interfaces
{
    public interface ICajaRegistradora
    {
        Resultado Cobrar(Venta venta, decimal dineroRecibido, Cajero cajero);
    }
}
