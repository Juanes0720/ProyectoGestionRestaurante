using Aplicaciones.Interfaces;
using Infraestructura.Repositorios;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructura.Extensiones
{
    public static class InfraestructuraExtension
    {
        // Repositorios en memoria: Singleton (si no, la lista se vacía en cada petición).
        public static IServiceCollection AgregarInfraestructura(this IServiceCollection services)
        {
            services.AddSingleton<IEmpleadoRepositorio, EmpleadoRepositorioMemoria>();
            services.AddSingleton<IRegistroHorasRepositorio, RegistroHorasRepositorioMemoria>();
            services.AddSingleton<IProductoRepositorio, ProductoRepositorioMemoria>();
            services.AddSingleton<IVentaRepositorio, VentaRepositorioMemoria>();
            services.AddSingleton<IPedidoRepositorio, PedidoRepositorioMemoria>();
            services.AddSingleton<IAperturaCajaRepositorio, AperturaCajaRepositorioMemoria>();
            services.AddSingleton<IFacturaRepositorio, FacturaRepositorioMemoria>();
            services.AddSingleton<IGastoRepositorio, GastoRepositorioMemoria>();
            services.AddSingleton<ICorteRepositorio, CorteRepositorioMemoria>();
            return services;
        }
    }
}
