using Aplicaciones.DTOs;
using Modelos;

namespace Aplicaciones.Interfaces
{
    public interface ICorteCajaServicio
    {
        Resultado HacerCorte(bool cerrarCaja);
        Resultado Listar();
    }
}
