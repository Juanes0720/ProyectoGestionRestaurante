using Aplicaciones.DTOs;
using Modelos;

namespace Aplicaciones.Interfaces
{
    public interface IAuthServicio
    {
        Resultado Login(LoginEntrada entrada);
    }
}
