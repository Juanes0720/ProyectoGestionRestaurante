namespace Modelos.Empleados
{
    public abstract class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public abstract string Tipo { get; }

        // Nunca se guarda la contraseña: solo su hash.
        public string ContrasenaHash { get; set; } = string.Empty;
    }
}
