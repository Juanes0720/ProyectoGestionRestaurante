using Aplicaciones.DTOs;
using Modelos;

namespace Aplicaciones.Interfaces
{
    public interface IRegistroHorasServicio
    {
        Resultado RegistrarEntrada(RegistroHorasEntrada entrada);
        Resultado RegistrarSalida(RegistroHorasEntrada entrada);
        Resultado ConsultarHoras(int empleadoId);
    }
}
