using Modelos.Empleados;

namespace Aplicaciones.Interfaces
{
    // La capa Aplicación solo pide "dame un token"; cómo se fabrica (JWT) lo decide la API.
    public interface IGeneradorToken
    {
        string Generar(Empleado empleado);
    }
}
