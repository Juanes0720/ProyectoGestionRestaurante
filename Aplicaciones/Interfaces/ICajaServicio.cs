using Aplicaciones.DTOs;
using Modelos;

namespace Aplicaciones.Interfaces
{
    public interface ICajaServicio
    {
        Resultado Abrir(AbrirCajaEntrada entrada, int cajeroId);
        Resultado Cobrar(CobrarEntrada entrada, int cajeroId);
        Resultado ConsultarSaldo();
        Resultado ConsultarFactura(int numero);
    }
}
