namespace Aplicaciones.Servicios
{
    // Separa un DateTime en fecha y hora (la hora sin milisegundos) para las respuestas.
    public static class FormatoFecha
    {
        public static DateOnly SoloFecha(DateTime fecha)
        {
            return DateOnly.FromDateTime(fecha);
        }

        public static TimeOnly SoloHora(DateTime fecha)
        {
            return new TimeOnly(fecha.Hour, fecha.Minute, fecha.Second);
        }
    }
}
