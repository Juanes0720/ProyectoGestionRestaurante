using Aplicaciones.DTOs;
using Aplicaciones.Interfaces;
using Modelos;
using Modelos.Ventas;

namespace Aplicaciones.Servicios
{
    public class ProductoServicio : IProductoServicio
    {
        private readonly IProductoRepositorio _repositorio;

        public ProductoServicio(IProductoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public Resultado Crear(ProductoEntrada entrada)
        {
            var error = Validar(entrada);
            if (error != null) return Resultado.Error(error);

            var producto = new Producto
            {
                Nombre = entrada.Nombre.Trim(),
                Precio = entrada.Precio
            };
            _repositorio.Agregar(producto);
            return Resultado.Ok(Mapear(producto), "Producto creado.");
        }

        public Resultado Editar(int id, ProductoEntrada entrada)
        {
            var producto = _repositorio.ObtenerPorId(id);
            if (producto == null)
                return Resultado.Error("El producto no existe.");

            var error = Validar(entrada);
            if (error != null) return Resultado.Error(error);

            producto.Nombre = entrada.Nombre.Trim();
            producto.Precio = entrada.Precio;
            return Resultado.Ok(Mapear(producto), "Producto actualizado.");
        }

        public Resultado Listar()
        {
            var lista = _repositorio.ObtenerTodos().Select(Mapear).ToList();
            return Resultado.Ok(lista);
        }

        // Devuelve el mensaje de error, o null si todo está bien.
        private static string? Validar(ProductoEntrada entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada.Nombre))
                return "El nombre es obligatorio.";
            if (entrada.Precio <= 0)
                return "El precio debe ser mayor que cero.";
            return null;
        }

        private static ProductoSalida Mapear(Producto p)
        {
            return new ProductoSalida { Id = p.Id, Nombre = p.Nombre, Precio = p.Precio };
        }
    }
}
