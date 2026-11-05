using System.Security.Cryptography;

namespace Aplicaciones.Servicios
{
    // Guarda las contraseñas como hash con sal (PBKDF2), nunca en texto plano.
    public static class Contrasenas
    {
        private const int Iteraciones = 100_000;

        public static string Hashear(string contrasena)
        {
            var sal = RandomNumberGenerator.GetBytes(16);
            var hash = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, Iteraciones, HashAlgorithmName.SHA256, 32);
            return Convert.ToBase64String(sal) + ":" + Convert.ToBase64String(hash);
        }

        public static bool Verificar(string contrasena, string guardado)
        {
            var partes = guardado.Split(':');
            if (partes.Length != 2) return false;

            var sal = Convert.FromBase64String(partes[0]);
            var esperado = Convert.FromBase64String(partes[1]);
            var hash = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, Iteraciones, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(hash, esperado);
        }
    }
}
