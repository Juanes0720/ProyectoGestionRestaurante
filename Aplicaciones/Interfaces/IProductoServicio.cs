using Aplicaciones.DTOs;
using Modelos;

namespace Aplicaciones.Interfaces
{
    public interface IProductoServicio
    {
        Resultado Crear(ProductoEntrada entrada);
        Resultado Editar(int id, ProductoEntrada entrada);
        Resultado Listar();
    }
}
