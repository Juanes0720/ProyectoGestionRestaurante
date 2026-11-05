namespace Modelos
{

    public class Resultado
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public object? Datos { get; set; }

        public static Resultado Ok(object? datos = null, string mensaje = "")
        {
            return new Resultado { Exito = true, Mensaje = mensaje, Datos = datos };
        }

        public static Resultado Error(string mensaje)
        {
            return new Resultado { Exito = false, Mensaje = mensaje };
        }
    }
}
