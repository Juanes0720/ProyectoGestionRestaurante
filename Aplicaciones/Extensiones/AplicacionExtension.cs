using Aplicaciones.Interfaces;
using Aplicaciones.Servicios;
using Microsoft.Extensions.DependencyInjection;

namespace Aplicaciones.Extensiones
{
    public static class AplicacionExtension
    {
        // Servicios: Scoped (uno por petición).
        public static IServiceCollection AgregarAplicacion(this IServiceCollection services)
        {
            services.AddScoped<IEmpleadoServicio, EmpleadoServicio>();
            services.AddScoped<IAuthServicio, AuthServicio>();
            services.AddScoped<IRegistroHorasServicio, RegistroHorasServicio>();
            services.AddScoped<IProductoServicio, ProductoServicio>();
            services.AddScoped<IVentaServicio, VentaServicio>();
            services.AddScoped<IPedidoServicio, PedidoServicio>();
            services.AddScoped<ICajaServicio, CajaServicio>();
            services.AddScoped<ICajaRegistradora, CajaRegistradora>();
            services.AddScoped<IGastoServicio, GastoServicio>();
            services.AddScoped<ICorteCajaServicio, CorteCajaServicio>();
            return services;
        }
    }
}
