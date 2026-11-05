using Modelos;

namespace Aplicaciones.Interfaces
{
    public interface IVentaServicio
    {
        // Las ventas se crean desde el pedido; aquí solo se consultan.
        Resultado Consultar(int id);
    }
}
