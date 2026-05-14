using System.Security.Cryptography;
using System.Text;

namespace MateAventuras_Corregido.Helpers
{
    public static class Sha256Helper
    {
        /// <summary>
        /// Calcula el hash SHA256 de un string y lo devuelve como string hexadecimal
        /// </summary>
        /// <param name="input">Texto a hashear</param>
        /// <returns>Hash SHA256 en formato hexadecimal</returns>
        public static string ComputeHash(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BytesToHex(hashBytes);
            }
        }

        /// <summary>
        /// Convierte un array de bytes a string hexadecimal de forma eficiente
        /// </summary>
        private static string BytesToHex(byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
    }
}
