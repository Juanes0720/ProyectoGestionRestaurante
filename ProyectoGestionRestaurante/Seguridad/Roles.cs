namespace ProyectoGestionRestaurante.Seguridad
{
    // Los nombres de rol son los mismos que Empleado.Tipo.
    public static class Roles
    {
        public const string Cajero = "Cajero";
        public const string Supervisor = "Supervisor";
        public const string Todos = "Cajero,Supervisor";   // cualquiera de los dos
    }
}
