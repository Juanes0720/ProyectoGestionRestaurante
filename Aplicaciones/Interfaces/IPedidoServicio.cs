using Aplicaciones.DTOs;
using Modelos;

namespace Aplicaciones.Interfaces
{
    public interface IPedidoServicio
    {
        Resultado Crear(PedidoEntrada entrada);   // crea el pedido Y su venta
        Resultado Actualizar(int id, PedidoEntrada entrada);
        Resultado Consultar(int id);
        Resultado Listar();
    }
}
